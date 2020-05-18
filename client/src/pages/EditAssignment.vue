<template>
  <q-page class="flex row justify-center content-start items-center q-col-gutter-lg" padding>
    <div class="row justify-center col-12">
      <div class="col-12 col-md-8 col-lg-4">
        <q-card class="q-pa-sm">
          <q-card-section>
            <span class="text-h5">Edit Assignment</span>
          </q-card-section>

          <q-card-section>
            <q-select v-model="assignment.Type" :options="assignmentTypes" label="Type"></q-select>
            <q-input
              v-model.number="assignment.Weight"
              label="Weight"
              type="number"
              min="0"
              max="1"
              step="0.05"
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
        :entity="assignmentTaskTable.entity"
        :data="assignmentTaskTable.items"
        :columns="assignmentTaskTable.columns"
        :pagination="assignmentTaskTable.pagination"
        :actions="assignmentTaskTable.actions"
      >
        <template v-slot:form="{ item }">
          <q-input v-model.number="item.Number" label="Number" type="number"></q-input>
          <q-input
            v-model.number="item.Weight"
            label="Weight"
            type="number"
            min="0"
            max="1"
            step="0.01"
          ></q-input>
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
  name: 'EditAssignmentPage',
  components: {
    OCrudTable
  },
  setup (props, context) {
    const assignmentTaskTable = useAssignmentTaskTable(context)

    const { courseId, assignmentId } = context.root.$route.params
    const { loading, onUpdate, assignment } = useUpdateForm(courseId, assignmentId)

    const assignmentTypes = ['Midterm', 'Final', 'Project', 'LabExam', 'Assignment', 'Other']

    return {
      assignmentTaskTable: ref(assignmentTaskTable),
      loading,
      onUpdate,
      assignment,
      assignmentTypes
    }
  }
})

function useUpdateForm (courseId, assignmentId) {
  const loading = ref(false)
  const assignment = reactive({
    Id: 0,
    CourseId: 0,
    Type: '',
    Weight: 1
  })

  const assignmentService = new ODataApiService(`/api/Courses/${courseId}/Assignments`)

  onMounted(async () => {
    loading.value = true
    try {
      const data = await assignmentService.get(assignmentId)
      Object.assign(assignment, data)
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
      await assignmentService.update(assignmentId, assignment)
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
    assignment
  }
}

function useAssignmentTaskTable (context) {
  const items = ref([])
  const columns = ref([
    {
      name: 'number',
      label: 'Number',
      field: 'Number',
      sortable: true,
      searchable: false
    },
    {
      name: 'weight',
      label: 'Weight',
      field: 'Weight',
      sortable: true,
      searchable: false
    },
    { name: 'actions', label: 'Actions', align: 'right' }
  ])
  const pagination = reactive({
    page: 1,
    sortBy: 'number',
    descending: false,
    rowsPerPage: context.root.$q.screen.xs ? 12 : 24,
    rowsNumber: 0
  })

  const { assignmentId, courseInfoId, courseId } = context.root.$route.params

  const assignmentTaskService = new ODataApiService(`/api/Courses/${courseId}/Assignments/${assignmentId}/AssignmentTasks`)

  const entity = {
    key: 'Id',
    name: 'AssignmentTask',
    displayName: (plural = false) => `Assignment Task${plural ? 's' : ''}`,
    route: (key = '') => `/course_info/${courseInfoId}/courses/${courseId}/assignments/${assignmentId}/assignment_task/${key}`,
    service: assignmentTaskService,
    defaultValue () {
      return {
        Id: 0,
        Number: 1,
        Weight: 1
      }
    }
  }

  const actions = {
    create: {
      icon: 'mdi-plus'
    },
    edit: {
      enabled: false
    },
    search: {
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
