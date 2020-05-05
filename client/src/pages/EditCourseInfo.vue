<template>
  <q-page class="flex row justify-center content-start items-center q-col-gutter-lg" padding>
    <div class="row justify-center col-md-12 col-lg-4">
      <div class="col-12">
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
    <o-crud-table
      class="q-my-lg col-md-12 col-lg-8"
      :entity="courseEntity"
      :data="courseItems"
      :columns="courseColumns"
      :pagination="coursePagination"
    >
      <template v-slot:form="{ item }">
        <q-select v-model="item.Semester" :options="semesters" label="Semester" />
        <q-input v-model.number="item.Year" label="Year" type="number"></q-input>
      </template>
    </o-crud-table>

    <o-crud-table
      class="q-my-lg col-md-12 col-lg-8"
      :entity="entity"
      :data="items"
      :columns="columns"
      :pagination="pagination"
      :actions="actionConfig"
    >
      <template v-slot:form="{ item }">
        <q-input v-model.number="item.Code" label="Code" type="number" min="1" max="255"></q-input>
        <q-input v-model="item.Description" label="Description" autogrow></q-input>
      </template>
    </o-crud-table>
  </q-page>
</template>

<script>
import { ref, reactive, onMounted } from '@vue/composition-api'
import axios from 'axios'
import { Notify } from 'quasar'

import OCrudTable from '../components/OCrudTable'
import { pushDataToServer } from '../services/ApiService'

export default {
  name: 'EditCourseInfoPage',
  components: {
    OCrudTable
  },
  setup (props, context) {
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

    const entity = {
      key: 'Id',
      name: 'LearningOutcome',
      displayName: (plural = false) => `Learning Outcome${plural ? 's' : ''}`,
      route: (key = '') => `/course_info/${courseInfoId}/outcomes/${key}`,
      apiRoute: (key = '') => `CourseInfo/${courseInfoId}/Outcomes/${key}`,
      defaultValue () {
        return {
          Id: 0,
          Code: 1,
          Description: ''
        }
      }
    }

    const actionConfig = {
      create: {
        icon: 'mdi-plus'
      },
      edit: {
        enabled: false
      }
    }

    const semesters = ['Fall', 'Spring', 'Summer']

    const courseItems = ref([])

    const courseColumns = ref([
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
    const coursePagination = reactive({
      page: 1,
      sortBy: 'year',
      descending: true,
      rowsPerPage: context.root.$q.screen.xs ? 12 : 24,
      rowsNumber: 0
    })

    const courseEntity = {
      key: 'Id',
      name: 'Course',
      displayName: (plural = false) => `Course${plural ? 's' : ''}`,
      route: (key = '') => `/course_info/${courseInfoId}/course/${key}`,
      apiRoute: (key = '') => `CourseInfo/${courseInfoId}/Courses/${key}`,
      defaultValue () {
        return {
          Id: 0,
          Semester: '',
          Year: new Date().getFullYear()
        }
      }
    }

    const { loading, onUpdate, courseInfo } = useUpdateForm(courseInfoId)

    return {
      items,
      columns,
      pagination,
      entity,
      actionConfig,

      courseEntity,
      courseItems,
      courseColumns,
      coursePagination,

      loading,
      onUpdate,
      courseInfo,
      semesters
    }
  }
}

function useUpdateForm (courseInfoId) {
  const loading = ref(false)

  const courseInfo = reactive({
    Id: 0,
    Code: '',
    Name: '',
    Credit: 1,
    DepartmentId: 1
  })

  const path = `CourseInfo/${courseInfoId}`

  onMounted(async () => {
    loading.value = true

    try {
      const { data } = await axios(`/api/${path}`)
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
      await pushDataToServer(path, courseInfo, true)
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
</script>
