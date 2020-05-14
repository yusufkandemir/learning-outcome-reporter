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
              @input="onFileInput"
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
            <div class="row" v-for="worksheet in worksheets" :key="worksheet.name">
              <div class="row col-12 q-mb-sm">
                <div class="col-12 col-sm-6 col-md-4">
                  <q-input
                    v-model.number="worksheetToFieldMapping[worksheet.name].assignmentId"
                    type="number"
                    filled
                    :label="worksheet.name"
                    @input="value => refreshOptions(worksheet.name, value)"
                  />
                </div>
              </div>

              <div
                class="row col-12 q-col-gutter-xs q-pa-sm"
                v-if="worksheetToFieldMapping[worksheet.name].assignmentId !== null"
              >
                <div
                  class="col-6 col-sm-4 col-md-3"
                  v-for="column in worksheet.columns"
                  :key="column"
                >
                  <q-select
                    filled
                    v-model="worksheetToFieldMapping[worksheet.name].columns[column]"
                    :label="column"
                    :options="fieldsToMap[worksheet.name]"
                    emit-value
                    map-options
                    :loading="optionsLoading"
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
import { defineComponent, ref, reactive, computed } from '@vue/composition-api'
import { Workbook } from 'exceljs'

import OCrudTable from '../components/OCrudTable'
import { ODataApiService } from '../services/ApiService'

export default defineComponent({
  name: 'EditCoursePage',
  components: {
    OCrudTable
  },
  setup (props, context) {
    const loading = ref(false)
    const step = ref(1)

    const file = ref(null)

    const submit = () => {
      //
    }

    const worksheets = ref([])

    // worksheetToFieldMapping.worksheets[worksheet.name].tasks[task.number]
    const { worksheetToFieldMapping, fieldsToMap, baseFieldsToMap } = createMappings(worksheets)

    const form = reactive({
      CourseInfoId: null,
      CourseId: null
    })

    const onFileInput = () => {
      //
    }

    const optionsLoading = ref(false)

    const refreshOptions = async (worksheetName, assignmentId) => {
      const assignmentTaskService = new ODataApiService(`/api/Course/${form.CourseId}/Assignments/${assignmentId}/AssignmentTasks`)

      optionsLoading.value = true

      const assignmentTasks = await assignmentTaskService.getAll({ startRow: 0, count: 100 })

      const extraFieldsToMap = assignmentTasks.items.map(assignmentTask => ({
        label: `Task #${assignmentTask.Number} - ${assignmentTask.Weight}`,
        value: assignmentTask.Id
      }))

      fieldsToMap[worksheetName] = [
        ...baseFieldsToMap,
        ...extraFieldsToMap
      ]

      optionsLoading.value = false
    }

    const { loadFile } = useExcel()

    const handleFilePick = async () => {
      const data = await loadFile(file.value)

      worksheets.value = [...data]

      step.value = 2
    }

    const finishMapping = () => {
      const payload = {
        courseId: form.CourseId,
        studentResults: {
          // '1700001234': {
          //   name: 'John Doe',
          //   assignmentTaskResults: {
          //     // assignmentId : results
          //     0: {
          //       // assignmentTaskId : grade
          //       1: 100,
          //       5: 76
          //     },
          //     1: {
          //       1: 85,
          //       2: 71
          //     }
          //   }
          // }
        }
      }

      for (const [worksheetName, worksheetMapping] of Object.entries(worksheetToFieldMapping.value)) {
        const worksheet = worksheets.value.find(x => x.name === worksheetName)
        const { assignmentId, columns } = worksheetMapping

        const reverseMapping = {}

        for (const [column, property] of Object.entries(columns)) {
          if (property === 'none') {
            continue
          }

          reverseMapping[property] = column
        }

        if (Object.keys(reverseMapping).length === 0) {
          continue
        }

        worksheet.values.forEach(value => {
          const studentId = value[reverseMapping.studentId]
          const studentName = value[reverseMapping.studentName]

          if (payload.studentResults[studentId] === undefined) {
            payload.studentResults[studentId] = {
              name: studentName,
              assignmentTaskResults: {}
            }
          }

          const assignmentTaskResults = {}

          for (const [property, column] of Object.entries(reverseMapping)) {
            // assignmentTaskId
            if (property !== 'studentId' && property !== 'studentName') {
              assignmentTaskResults[property] = value[column]
            }
          }

          payload.studentResults[studentId].assignmentTaskResults[assignmentId] = assignmentTaskResults
        })
      }

      console.log(payload)
    }

    return {
      step,
      loading,

      file,
      submit,

      form,
      onFileInput,

      worksheets,
      worksheetToFieldMapping,
      fieldsToMap,
      refreshOptions,
      optionsLoading,

      handleFilePick,
      finishMapping
    }
  }
})

function createMappings (worksheets) {
  const baseFieldsToMap = [
    { label: 'Do not map', value: 'none' },
    { label: 'Student Id', value: 'studentId' },
    { label: 'Student Name', value: 'studentName' }
  ]

  const fieldsToMap = reactive({})

  const worksheetToFieldMapping = computed(() => {
    const mapping = reactive({})

    for (const worksheet of worksheets.value) {
      Vue.set(mapping, worksheet.name, {
        assignmentId: null,
        columns: {}
      })

      worksheet.columns.forEach(column => {
        Vue.set(mapping[worksheet.name].columns, column, 'none')

        fieldsToMap[worksheet.name] = []
      })
    }

    return mapping
  })

  return {
    baseFieldsToMap,
    fieldsToMap,
    worksheetToFieldMapping
  }
}

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
