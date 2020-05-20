<template>
  <q-page class="flex row justify-center content-start items-center q-col-gutter-lg" padding>
    <div class="row justify-center q-col-gutter-md col-12">
      <div class="col-12 col-md-8 col-lg-4">
        <q-card class="q-pa-sm">
          <q-card-section>
            <span class="text-h5">Edit Assignment Task</span>
          </q-card-section>

          <q-card-section>
            <q-input v-model.number="assignmentTask.Number" label="Number" type="number"></q-input>
            <q-input
              v-model.number="assignmentTask.Weight"
              label="Weight"
              type="number"
              min="0"
              max="1"
              step="0.01"
            ></q-input>
          </q-card-section>

          <q-card-actions class="justify-end">
            <q-btn color="primary" @click="onUpdate" :loading="loading">Update</q-btn>
          </q-card-actions>
        </q-card>
      </div>

      <div class="col-12 col-md-8 col-lg-4">
        <q-card class="q-pa-sm">
          <q-card-section>
            <span class="text-h5">Outcomes</span>
          </q-card-section>

          <q-card-section>
            <o-entity-selector
              v-model="learningOutcomes.items"
              multiple
              emit-key
              live-edit
              @selection="learningOutcomes.onSelection"
              :columns="learningOutcomes.columns"
              :entity="learningOutcomes.entity"
              :display="model => model.Code"
            />

            <o-entity-selector
              v-if="programOutcomes.isInitialized"
              v-model="programOutcomes.items"
              emit-key
              multiple
              live-edit
              @selection="programOutcomes.onSelection"
              :columns="programOutcomes.columns"
              :entity="programOutcomes.entity"
              :display="model => model.Code"
            />
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive, onMounted, watch } from '@vue/composition-api'
import { Notify } from 'quasar'
import axios from 'axios'

import OEntitySelector from '../components/OEntitySelector'
import { ODataApiService } from '../services/ApiService'

export default defineComponent({
  name: 'EditAssignmentPage',
  components: {
    OEntitySelector
  },
  setup (props, context) {
    const { courseInfoId, courseId, assignmentId, taskId } = context.root.$route.params
    const { loading, onUpdate, assignmentTask } = useUpdateForm(courseId, assignmentId, taskId)

    const learningOutcomes = useLearningOutcomeSelector({ courseInfoId, courseId, assignmentId, taskId })
    const programOutcomes = useProgramOutcomeSelector({ courseInfoId, courseId, assignmentId, taskId })

    // Update outcomes list with current ones
    watch(() => assignmentTask.Outcomes, (assignmentTaskOutcomes, oldValue) => {
      const outcomeIds = [...assignmentTaskOutcomes.map(({ Outcome: outcome }) => outcome.Id)]

      learningOutcomes.items = outcomeIds
      programOutcomes.items = outcomeIds
    })

    return {
      loading,
      onUpdate,
      assignmentTask,

      learningOutcomes: ref(learningOutcomes),
      programOutcomes: ref(programOutcomes)
    }
  }
})

function useProgramOutcomeSelector ({ courseInfoId, courseId, assignmentId, taskId }) {
  // Used for avoiding a possible race condition
  const isInitialized = ref(false)
  const items = ref([])

  const columns = ref([
    {
      name: 'code',
      label: 'Code',
      field: 'Code',
      sortable: true
    },
    {
      name: 'description',
      label: 'Description',
      field: 'Description',
      sortable: true,
      searchable: true
    }
  ])

  const entity = reactive({
    key: 'Id',
    name: 'ProgramOutcome',
    displayName: (plural = false) => `Program Outcome${plural ? 's' : ''}`,
    route: (key = '') => `/departments/${key}`,
    service: null,
    defaultValue () {
      return {
        Id: 0,
        Code: null,
        Description: ''
      }
    }
  })

  onMounted(async () => {
    const courseInfoService = new ODataApiService('/api/CourseInfos')

    const { DepartmentId: departmentId } = await courseInfoService.get(courseInfoId, { $select: 'DepartmentId' })

    const programOutcomeService = new ODataApiService(`/api/Departments/${departmentId}/Outcomes`)

    entity.service = programOutcomeService

    isInitialized.value = true
  })

  const onSelection = async ({ keys, isAdded }) => {
    const promises = keys.map(key => axios(`/api/Courses/${courseId}/Assignments/${assignmentId}/AssignmentTasks/${taskId}/Outcomes/${key}`, { method: isAdded ? 'PUT' : 'DELETE' }))

    await Promise.all(promises)
  }

  return {
    isInitialized,
    items,

    columns,
    entity,

    onSelection
  }
}

function useLearningOutcomeSelector ({ courseInfoId, courseId, assignmentId, taskId }) {
  const items = ref([])

  const columns = ref([
    {
      name: 'code',
      label: 'Code',
      field: 'Code',
      sortable: true
    },
    {
      name: 'description',
      label: 'Description',
      field: 'Description',
      sortable: true,
      searchable: true
    }
  ])

  const service = new ODataApiService(`/api/CourseInfos/${courseInfoId}/Outcomes`)

  const entity = {
    key: 'Id',
    name: 'LearningOutcome',
    displayName: (plural = false) => `Learning Outcome${plural ? 's' : ''}`,
    route: (key = '') => `/course_info/${courseInfoId}`,
    service,
    defaultValue () {
      return {
        Id: 0,
        Code: null,
        Description: '',
        CourseInfoId: courseInfoId
      }
    }
  }

  const onSelection = async ({ keys, isAdded }) => {
    const promises = keys.map(key => axios(`/api/Courses/${courseId}/Assignments/${assignmentId}/AssignmentTasks/${taskId}/Outcomes/${key}`, { method: isAdded ? 'PUT' : 'DELETE' }))

    await Promise.all(promises)
  }

  return {
    items,

    columns,
    entity,

    onSelection
  }
}

function useUpdateForm (courseId, assignmentId, taskId) {
  const loading = ref(false)
  const assignmentTask = reactive({
    Id: 0,
    Number: 1,
    Weight: 1,
    Outcomes: []
  })

  const service = new ODataApiService(`/api/Courses/${courseId}/Assignments/${assignmentId}/AssignmentTasks`)

  onMounted(async () => {
    loading.value = true
    try {
      const data = await service.get(taskId, { $expand: 'Outcomes($expand=Outcome)' })
      Object.assign(assignmentTask, data)
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
      await service.update(taskId, assignmentTask)
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
    assignmentTask
  }
}
</script>
