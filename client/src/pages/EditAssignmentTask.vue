<template>
  <q-page class="flex row justify-center content-start items-center q-col-gutter-lg" padding>
    <div class="row justify-center col-12">
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
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive, onMounted } from '@vue/composition-api'
import { Notify } from 'quasar'

import { ODataApiService } from '../services/ApiService'

export default defineComponent({
  name: 'EditAssignmentPage',
  setup (props, context) {
    const { courseId, assignmentId, taskId } = context.root.$route.params
    const { loading, onUpdate, assignmentTask } = useUpdateForm(courseId, assignmentId, taskId)

    return {
      loading,
      onUpdate,
      assignmentTask
    }
  }
})

function useUpdateForm (courseId, assignmentId, taskId) {
  const loading = ref(false)
  const assignmentTask = reactive({
    Id: 0,
    Number: 1,
    Weight: 1
  })

  const service = new ODataApiService(`/api/Courses/${courseId}/Assignments/${assignmentId}/AssignmentTasks`)

  onMounted(async () => {
    loading.value = true
    try {
      const data = await service.get(taskId)
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
