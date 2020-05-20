<template>
  <q-page class="flex row justify-center content-start items-center q-col-gutter-lg" padding>
    <div class="row justify-center col-12 q-mb-lg">
      <q-toolbar class="bg-grey-10 q-pa-sm">
        <q-space />

        <!-- TODO: Redirect to the import page with required parameters to provide a shortcut to form -->
        <q-btn color="primary" to="/import_excel">
          <q-icon name="mdi-microsoft-excel" left />Import Excel
        </q-btn>
      </q-toolbar>
    </div>

    <div class="row justify-center col-12 col-lg-4">
      <div class="col-12">
        <q-card class="q-pa-sm">
          <q-card-section>
            <span class="text-h5">Edit Course</span>
          </q-card-section>

          <q-card-section>
            <q-select v-model="course.Semester" :options="semesters" label="Semester" />
            <q-input v-model.number="course.Year" label="Year" type="number"></q-input>
          </q-card-section>

          <q-card-actions class="justify-end">
            <q-btn color="primary" @click="onUpdate" :loading="loading">Update</q-btn>
          </q-card-actions>
        </q-card>
      </div>
    </div>

    <div class="col-12 col-lg-8">
      <o-crud-table
        class="q-my-lg"
        :entity="assignmentTable.entity"
        :data="assignmentTable.items"
        :columns="assignmentTable.columns"
        :pagination="assignmentTable.pagination"
        :actions="assignmentTable.actions"
      >
        <template v-slot:form="{ item }">
          <q-select v-model="item.Type" :options="assignmentTable.assignmentTypes" label="Type"></q-select>
          <q-input v-model.number="item.Weight" type="number" min="0" max="1" label="Weight"></q-input>
        </template>
      </o-crud-table>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive, onMounted } from '@vue/composition-api'

import OCrudTable from '../components/OCrudTable'
import { ODataApiService } from '../services/ApiService'
import { Notify } from 'quasar'

export default defineComponent({
  name: 'EditCoursePage',
  components: {
    OCrudTable
  },
  setup (props, context) {
    const { courseInfoId, courseId } = context.root.$route.params
    const { loading, onUpdate, course } = useUpdateForm(courseInfoId, courseId)

    const semesters = ['Fall', 'Spring', 'Summer']

    const assignmentTable = useAssignmentTable(context)

    return {
      loading,
      onUpdate,
      course,
      semesters,

      assignmentTable: ref(assignmentTable)
    }
  }
})

function useUpdateForm (courseInfoId, courseId) {
  const loading = ref(false)

  const course = reactive({
    Id: 0,
    Semester: '',
    Year: new Date().getFullYear()
  })

  const courseService = new ODataApiService(`/api/CourseInfos/${courseInfoId}/Courses`)

  onMounted(async () => {
    loading.value = true

    try {
      const data = await courseService.get(courseId)
      Object.assign(course, data)
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
      await courseService.update(courseId, course)
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
    course
  }
}

function useAssignmentTable (context) {
  const items = ref([])
  const columns = ref([
    {
      name: 'type',
      label: 'Type',
      field: 'Type',
      sortable: false,
      searchable: false
    },
    {
      name: 'weight',
      label: 'Weight',
      field: 'Weight',
      sortable: false,
      searchable: false
    },
    { name: 'actions', label: 'Actions', align: 'right' }
  ])
  const pagination = reactive({
    page: 1,
    sortBy: 'type',
    descending: true,
    rowsPerPage: context.root.$q.screen.xs ? 12 : 24,
    rowsNumber: 0
  })

  const { courseId, courseInfoId } = context.root.$route.params

  const assignmentsService = new ODataApiService(`/api/Courses/${courseId}/Assignments`)
  const entity = {
    key: 'Id',
    name: 'Assignment',
    displayName: (plural = false) => `Assignment${plural ? 's' : ''}`,
    route: (key = '') => `/course_info/${courseInfoId}/courses/${courseId}/assignments/${key}`,
    service: assignmentsService,
    defaultValue () {
      return {
        Id: 0,
        Type: '',
        Weight: '1'
      }
    }
  }

  const actions = {
    create: {
      icon: 'mdi-clipboard-check'
    },
    search: {
      enabled: false
    }
  }

  const assignmentTypes = ['Midterm', 'Final', 'Project', 'LabExam', 'Assignment', 'Other']

  return {
    items,
    columns,
    pagination,
    entity,
    actions,

    assignmentTypes
  }
}
</script>
