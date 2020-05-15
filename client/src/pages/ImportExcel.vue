<template>
  <q-page class="flex row justify-center content-start items-center q-col-gutter-lg" padding>
    <div class="col-12 row justify-center">
      <div class="col-12 col-sm-10 col-lg-6 col-xl-4">
        <q-card class="full-width q-pa-sm">
          <q-card-section>
            <span class="text-h5">Import Excel</span>
          </q-card-section>

          <q-card-section class="row q-col-gutter-md">
            <div class="col-12 col-sm-6">
              <q-input
                filled
                v-model.number="form.CourseInfoId"
                label="Course Info Id"
                type="number"
                min="0"
              ></q-input>
            </div>

            <div class="col-12 col-sm-6">
              <q-input
                filled
                :disable="form.CourseInfoId == null"
                v-model.number="form.CourseId"
                label="Course Id"
                type="number"
                min="0"
              ></q-input>
            </div>
          </q-card-section>
        </q-card>
      </div>
    </div>

    <div class="col-12 row justify-center">
      <div class="col-12 col-md-10 col-lg-6">
        <q-stepper
          v-model="step"
          header-nav
          ref="stepper"
          color="primary"
          animated
          :contracted="$q.screen.xs"
        >
          <q-step
            :name="1"
            title="Pick a spreadsheet file"
            active-icon="mdi-cloud-upload"
            :done="step > 1"
            :header-nav="step > 1"
          >
            <q-file
              filled
              counter
              v-model="file"
              class="excel-file-picker"
              label="Pick a spreadsheet file"
              accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"
            >
              <template v-slot:prepend>
                <q-icon name="mdi-cloud-upload" />
              </template>
            </q-file>

            <q-stepper-navigation class="flex justify-end">
              <q-btn
                @click="handleFilePick"
                color="primary"
                label="Continue"
                :disabled="file === null || form.CourseId == null"
              />
            </q-stepper-navigation>
          </q-step>

          <q-step
            :name="2"
            title="Map the attributes"
            icon="mdi-graph"
            :done="step > 2"
            :header-nav="file !== null || step > 2"
          >
            <div class="row" v-for="assignment in assignments" :key="assignment.Id">
              <div class="row col-12 q-mb-sm">
                <div class="col-12 col-sm-6 col-md-4">
                  <q-select
                    v-model.number="fieldMapping[assignment.Id].worksheetName"
                    filled
                    :label="`${assignment.Type} #${assignment.Id} - ${assignment.Weight}`"
                    :options="worksheetOptions"
                    emit-value
                    map-options
                  />
                </div>
              </div>

              <div
                class="row col-12 q-col-gutter-xs q-pa-sm"
                v-if="fieldMapping[assignment.Id].worksheetName !== 'none'"
              >
                <div class="col-6 col-sm-4 col-md-3">
                  <q-select
                    filled
                    v-model="fieldMapping[assignment.Id].columns.studentId"
                    label="Student Id"
                    :options="columnsToMap[fieldMapping[assignment.Id].worksheetName]"
                    emit-value
                    map-options
                  />
                </div>
                <div class="col-6 col-sm-4 col-md-3">
                  <q-select
                    filled
                    v-model="fieldMapping[assignment.Id].columns.studentName"
                    label="Student Name"
                    :options="columnsToMap[fieldMapping[assignment.Id].worksheetName]"
                    emit-value
                    map-options
                  />
                </div>
                <div
                  class="col-6 col-sm-4 col-md-3"
                  v-for="field in taskFieldsToMap[assignment.Id]"
                  :key="field.value"
                >
                  <q-select
                    filled
                    v-model="fieldMapping[assignment.Id].columns.tasks[field.value]"
                    :label="field.label"
                    :options="columnsToMap[fieldMapping[assignment.Id].worksheetName]"
                    emit-value
                    map-options
                  />
                </div>
              </div>

              <div class="col-12">
                <q-separator spaced />
              </div>
            </div>

            <q-stepper-navigation class="flex justify-end">
              <q-btn flat @click="step = 1" color="primary" label="Back" class="q-mr-sm" />

              <q-btn @click="finishMapping" color="primary" label="Continue" />
            </q-stepper-navigation>
          </q-step>

          <q-step :name="3" title="Finalize" icon="mdi-file-find" :header-nav="step > 3">
            <div>Press finish to import the data, it can take a while</div>

            <q-stepper-navigation class="flex justify-end">
              <q-btn flat @click="step = 2" color="primary" label="Back" class="q-mr-sm" />

              <q-btn color="primary" @click="submit" label="Finish" />
            </q-stepper-navigation>
          </q-step>
        </q-stepper>
      </div>
    </div>
  </q-page>
</template>

<script>
import Vue from 'vue'
import { defineComponent, ref, reactive } from '@vue/composition-api'
import { Workbook } from 'exceljs'

import OCrudTable from '../components/OCrudTable'
import { ODataApiService } from '../services/ApiService'

import axios from 'axios'
import { Notify, Loading, QSpinnerGears } from 'quasar'

export default defineComponent({
  name: 'EditCoursePage',
  components: {
    OCrudTable
  },
  setup (props, context) {
    const loading = ref(false)
    const step = ref(1)

    const file = ref(null)

    const worksheets = ref([])
    const worksheetOptions = ref([])

    const fieldMapping = reactive({})
    const columnsToMap = reactive({})
    const taskFieldsToMap = reactive({})

    const form = reactive({
      CourseInfoId: null,
      CourseId: null
    })

    const assignments = ref([])

    const { loadFile } = useExcel()

    const handleFilePick = async () => {
      const data = await loadFile(file.value)

      worksheets.value = [...data]
      worksheetOptions.value = [
        { label: 'Do not map', value: 'none' }
      ]

      for (const worksheet of worksheets.value) {
        columnsToMap[worksheet.name] = [
          { label: 'Do not map', value: 'none' },
          ...worksheet.columns
        ]

        worksheetOptions.value.push(worksheet.name)
      }

      const assignmentService = new ODataApiService(`/api/Course/${form.CourseId}/Assignments`)
      const { items } = await assignmentService.getAll({ parameters: { $expand: 'AssignmentTasks' } })

      assignments.value = items

      for (const assignment of assignments.value) {
        const tasks = {}
        for (const assignmentTask of assignment.AssignmentTasks) {
          tasks[assignmentTask.Id] = 'none'

          if (taskFieldsToMap[assignment.Id] === undefined) {
            taskFieldsToMap[assignment.Id] = []
          }

          taskFieldsToMap[assignment.Id].push({
            label: `Task #${assignmentTask.Number} - ${assignmentTask.Weight}`,
            value: assignmentTask.Id
          })
        }

        Vue.set(fieldMapping, assignment.Id, {
          worksheetName: 'none',
          columns: {
            studentId: 'none',
            studentName: 'none',
            tasks
          }
        })
      }

      step.value = 2
    }

    const payload = {
      courseId: null,
      studentResults: {}
    }

    const finishMapping = () => {
      payload.courseId = form.CourseId

      for (const [assignmentId, worksheetMapping] of Object.entries(fieldMapping)) {
        const { worksheetName, columns } = worksheetMapping
        const worksheet = worksheets.value.find(x => x.name === worksheetName)

        if (columns.studentId === 'none' || columns.studentName === 'none') {
          continue
        }

        worksheet.values.forEach(value => {
          const studentId = value[columns.studentId]
          const studentName = value[columns.studentName]

          if (payload.studentResults[studentId] === undefined) {
            payload.studentResults[studentId] = {
              name: studentName,
              assignmentTaskResults: {}
            }
          }

          const assignmentTaskResults = {}

          for (const [assignmentTaskId, column] of Object.entries(columns.tasks)) {
            if (column === 'none') {
              continue
            }

            const grade = value[column]
            assignmentTaskResults[assignmentTaskId] = grade
          }

          payload.studentResults[studentId].assignmentTaskResults[assignmentId] = assignmentTaskResults
        })
      }

      step.value = 3
    }

    const submit = async () => {
      Loading.show({
        message: 'The data is being imported to the system. This can take a while...',
        spinner: QSpinnerGears
      })

      try {
        await axios.post('/api/ImportExcel', payload)

        Notify.create({
          type: 'positive',
          message: 'Data is successfully imported'
        })

        context.root.$router.push(`/course_info/${form.CourseInfoId}/courses/${form.CourseId}`)
      } catch (error) {
        Notify.create({
          type: 'negative',
          message: 'An error occured while importing the data',
          caption: error.message
        })
      } finally {
        Loading.hide()
      }
    }

    return {
      step,
      loading,

      file,
      submit,

      form,

      worksheets,
      worksheetOptions,
      fieldMapping,
      columnsToMap,
      taskFieldsToMap,
      assignments,

      handleFilePick,
      finishMapping
    }
  }
})

function useExcel () {
  const loadFile = async (file) => {
    const arrayBuffer = await file.arrayBuffer()

    const workbook = new Workbook()
    await workbook.xlsx.load(arrayBuffer)

    const results = []

    workbook.eachSheet(sheet => {
      const name = sheet.name
      const [, columns, ...rows] = sheet.getSheetValues().map(row => row.slice(1))

      const values = []
      for (const row of rows) {
        const value = {}
        for (const [index, column] of columns.entries()) {
          value[column] = row[index]
        }

        values.push(value)
      }

      results.push({
        name,
        columns,
        values
      })
    })

    return results
  }

  return {
    loadFile
  }
}
</script>

<style lang="sass">
.excel-file-picker
  .q-field__control-container
    height: 150px
</style>
