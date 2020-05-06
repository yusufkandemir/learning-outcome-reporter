<template>
  <q-page class="flex row justify-center content-start items-center q-col-gutter-lg" padding>
    <div class="row justify-center col-12">
      <div class="col-12 col-md-8 col-lg-4">
        <q-card class="q-pa-sm">
          <q-card-section>
            <span class="text-h5">Edit Course Information</span>
          </q-card-section>

          <q-card-section>
            <q-input
              v-model.number="courseInfo.DepartmentId"
              label="Deparment Id"
              type="number"
              min="1"
              :loading="loading"
            ></q-input>
            <q-input v-model="courseInfo.Name" label="Name" :loading="loading"></q-input>
            <q-input v-model="courseInfo.Code" label="Course Code" :loading="loading"></q-input>
            <q-input
              v-model.number="courseInfo.Credit"
              type="number"
              min="1"
              label="Credit"
              :loading="loading"
            ></q-input>
          </q-card-section>

          <q-card-actions class="justify-end">
            <q-btn color="primary" @click="onUpdate" :loading="loading">Update</q-btn>
          </q-card-actions>
        </q-card>
      </div>
    </div>

    <div class="row justify-center col-12">
      <o-crud-table
        class="q-my-lg col-12 col-md-10 col-lg-8"
        :entity="courseTable.entity"
        :data="courseTable.items"
        :columns="courseTable.columns"
        :pagination="courseTable.pagination"
      >
        <template v-slot:form="{ item }">
          <q-select v-model="item.Semester" :options="courseTable.semesters" label="Semester" />
          <q-input v-model.number="item.Year" label="Year" type="number"></q-input>
        </template>
      </o-crud-table>
    </div>

    <div class="row justify-center col-12">
      <o-crud-table
        class="q-my-lg col-12 col-md-10 col-lg-8"
        :entity="learningOutcomeTable.entity"
        :data="learningOutcomeTable.items"
        :columns="learningOutcomeTable.columns"
        :pagination="learningOutcomeTable.pagination"
        :actions="learningOutcomeTable.actions"
      >
        <template v-slot:form="{ item }">
          <q-input v-model.number="item.Code" label="Code" type="number" min="1" max="255"></q-input>
          <q-input v-model="item.Description" label="Description" autogrow></q-input>
        </template>
      </o-crud-table>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive, onMounted } from '@vue/composition-api'
import { Notify } from 'quasar'

import OCrudTable from '../components/OCrudTable'
import { ODataApiService } from '../services/ApiService'

export default defineComponent({
  name: 'EditCourseInfoPage',
  components: {
    OCrudTable
  },
  setup (props, context) {
    const courseInfoId = context.root.$route.params.id

    const courseTable = useCourseTable(courseInfoId, context)

    const learningOutcomeTable = useLearningOutcomeTable(context)

    const { loading, onUpdate, courseInfo } = useUpdateForm(courseInfoId)

    return {
      courseTable: ref(courseTable),
      learningOutcomeTable: ref(learningOutcomeTable),

      loading,
      onUpdate,
      courseInfo
    }
  }
})

function useUpdateForm (courseInfoId) {
  const loading = ref(false)

  const courseInfo = reactive({
    Id: 0,
    Code: '',
    Name: '',
    Credit: 1,
    DepartmentId: 1
  })

  const courseInfoService = new ODataApiService('/api/CourseInfo')

  onMounted(async () => {
    loading.value = true

    try {
      const data = await courseInfoService.get(courseInfoId)
      Object.assign(courseInfo, data)
    } catch (error) {
      Notify.create({
        type: 'negative',
        position: 'top',
        message: 'An error occured while fetching the data',
        caption: error.message
      })

      return
    } finally {
      loading.value = false
    }
  })

  const onUpdate = async () => {
    loading.value = true

    try {
      await courseInfoService.update(courseInfoId, courseInfo)
    } catch (error) {
      Notify.create({
        type: 'negative',
        position: 'top',
        message: 'An error occured while updating',
        caption: error.message
      })

      return
    } finally {
      loading.value = false
    }

    Notify.create({
      type: 'positive',
      position: 'top',
      message: 'Update successful'
    })
  }

  return {
    loading,
    onUpdate,
    courseInfo
  }
}

function useCourseTable (courseInfoId, context) {
  const semesters = ['Fall', 'Spring', 'Summer']

  const items = ref([])
  const columns = ref([
    {
      name: 'semester',
      label: 'Semester',
      field: 'Semester',
      sortable: true,
      searchable: true
    },
    {
      name: 'year',
      label: 'Year',
      field: 'Year',
      sortable: true,
      searchable: true
    },
    { name: 'actions', label: 'Actions', align: 'right' }
  ])
  const pagination = reactive({
    page: 1,
    sortBy: 'year',
    descending: true,
    rowsPerPage: context.root.$q.screen.xs ? 12 : 24,
    rowsNumber: 0
  })

  const courseService = new ODataApiService(`/api/CourseInfo/${courseInfoId}/Courses`)

  const entity = {
    key: 'Id',
    name: 'Course',
    displayName: (plural = false) => `Course${plural ? 's' : ''}`,
    route: (key = '') => `/course_info/${courseInfoId}/courses/${key}`,
    service: courseService,
    defaultValue () {
      return {
        Id: 0,
        Semester: '',
        Year: new Date().getFullYear()
      }
    }
  }

  return {
    semesters,

    items,
    columns,
    pagination,
    entity
  }
}

function useLearningOutcomeTable (context) {
  const items = ref([])
  const columns = ref([
    {
      name: 'code',
      label: 'Code',
      field: 'Code',
      sortable: true,
      searchable: true
    },
    {
      name: 'description',
      label: 'Description',
      field: 'Description',
      sortable: true,
      searchable: true
    },
    { name: 'actions', label: 'Actions', align: 'right' }
  ])
  const pagination = reactive({
    page: 1,
    sortBy: 'code',
    descending: false,
    rowsPerPage: context.root.$q.screen.xs ? 12 : 24,
    rowsNumber: 0
  })
  const courseInfoId = context.root.$route.params.id

  const learningOutcomeService = new ODataApiService(`/api/CourseInfo/${courseInfoId}/Outcomes`)

  const entity = {
    key: 'Id',
    name: 'LearningOutcome',
    displayName: (plural = false) => `Learning Outcome${plural ? 's' : ''}`,
    route: (key = '') => `/course_info/${courseInfoId}/outcomes/${key}`,
    service: learningOutcomeService,
    defaultValue () {
      return {
        Id: 0,
        Code: 1,
        Description: ''
      }
    }
  }

  const actions = {
    create: {
      icon: 'mdi-plus'
    },
    edit: {
      enabled: false
    }
  }

  return {
    items,
    columns,
    pagination,
    entity,
    actions
  }
}
</script>
