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
              <q-btn @click="step = 2" color="primary" label="Continue" :disabled="file === null" />
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
                  :key="column.key"
                >
                  <q-select
                    filled
                    v-model="worksheetToFieldMapping[worksheet.name].columns[column.key]"
                    :label="column.header"
                    :options="fieldsToMap"
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

              <q-btn @click="step = 3" color="primary" label="Continue" />
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
import { defineComponent, ref, reactive, watch } from '@vue/composition-api'

import OCrudTable from '../components/OCrudTable'

export default defineComponent({
  name: 'EditCoursePage',
  components: {
    OCrudTable
  },
  setup (props, context) {
    const loading = ref(false)
    const step = ref(1)

    const file = ref(null)
    watch(file, value => {
      if (value !== null) {
        step.value = 2
      }
    })

    const submit = () => {
      //
    }

    const worksheets = ref([
      {
        name: 'Midterm',
        columns: [
          { header: 'Student Id', key: 'id' },
          { header: 'Student Name', key: 'name' },
          { header: 'Q1', key: 'q1' },
          { header: 'Q2', key: 'q2' }
        ]
      },
      {
        name: 'Final',
        columns: [
          { header: 'Student Id', key: 'id' },
          { header: 'Student Name', key: 'name' },
          { header: 'Q1', key: 'q1' },
          { header: 'Q2', key: 'q2' },
          { header: 'Q3', key: 'q3' }
        ]
      }
    ])

    // worksheetToFieldMapping.worksheets[worksheet.name].tasks[task.number]
    const { worksheetToFieldMapping, fieldsToMap } = createMappings(worksheets.value)

    const form = reactive({
      CourseInfoId: null,
      CourseId: null
    })

    const onFileInput = () => {
      //
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
      fieldsToMap
    }
  }
})

function createMappings (worksheets) {
  // TODO: Fetch assignmentTasks related with assignmentId, then add them to this array
  const fieldsToMap = ref([
    { label: 'Do not map', value: 'none' },
    { label: 'Student Id', value: 'studentId' },
    { label: 'Student Name', value: 'studentName' }
  ])

  const worksheetToFieldMapping = {}

  for (const worksheet of worksheets) {
    worksheetToFieldMapping[worksheet.name] = {
      assignmentId: null,
      columns: {}
    }

    worksheet.columns.forEach(column => {
      worksheetToFieldMapping[worksheet.name].columns[column.key] = 'none'
    })
  }

  return {
    fieldsToMap,
    worksheetToFieldMapping: reactive(worksheetToFieldMapping)
  }
}
</script>

<style lang="sass">
.excel-file-picker
  .q-field__control-container
    height: 150px
</style>
