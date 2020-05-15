<template>
  <q-page class="flex row justify-center content-start items-center q-col-gutter-lg" padding>
    <div class="row justify-center col-12">
      <div class="col-12 col-md-8 col-lg-4">
        <q-card class="q-pa-sm">
          <q-card-section>
            <span class="text-h5">Department Report</span>
          </q-card-section>

          <q-card-section>
            <o-entity-selector
              v-model="form.department"
              :columns="department.columns"
              :entity="department.entity"
              :display="model => model.Name"
            />

            <q-select v-model="form.semester" :options="semesters" label="Semester" />
            <q-input v-model.number="form.year" label="Year" type="number"></q-input>
          </q-card-section>

          <q-card-actions class="justify-end">
            <q-btn color="primary" @click="onGenerate" :loading="loading">Generate</q-btn>
          </q-card-actions>
        </q-card>
      </div>
    </div>

    <div class="row justify-center col-12">
      <div class="col-6" v-if="showingChart">
        <apexchart type="bar" :options="options" :series="series"></apexchart>
        <q-inner-loading :showing="loading">
          <q-spinner-gears size="xl" color="primary" />
        </q-inner-loading>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive } from '@vue/composition-api'
import axios from 'axios'
import { Notify } from 'quasar'

import OEntitySelector from '../components/OEntitySelector'
import { ODataApiService } from '../services/ApiService'

export default defineComponent({
  name: 'ReportPage',
  components: {
    OEntitySelector
  },
  setup (props, context) {
    const loading = ref(false)

    const showingChart = ref(false)
    const series = ref([{
      name: 'Avg. Performance',
      data: []
    }])

    const options = {
      colors: ['#FCCF31', '#17ead9', '#f02fc2'],
      grid: {
        show: true,
        strokeDashArray: 0,
        xaxis: {
          lines: {
            show: true
          }
        }
      },
      fill: {
        type: 'gradient',
        gradient: {
          shade: 'dark',
          type: 'vertical',
          shadeIntensity: 0.5,
          inverseColors: false,
          opacityFrom: 1,
          opacityTo: 0.8,
          stops: [0, 100]
        }
      },
      stroke: {
        width: 0
      },
      xaxis: {
        labels: {
          formatter: function (value) {
            return 'PO-' + value
          }
        }
      },
      yaxis: {
        title: {
          text: 'Avg. Performance (%)'
        }
      },
      title: {
        text: 'Department of Computer Engineering',
        align: 'center',
        floating: true
      },
      theme: {
        mode: 'dark'
      }
    }

    const form = reactive({
      department: null,
      semester: null,
      year: null
    })

    const onGenerate = async () => {
      loading.value = true

      try {
        const params = new URLSearchParams({
          DepartmentId: form.department.Id,
          Semester: form.semester,
          Year: form.year
        })

        const response = await axios.get(`/api/Reports/Department?${params.toString()}`)

        series.value = [{
          name: 'Avg. Performance',
          data: response.data.results
        }]

        showingChart.value = true
      } catch (error) {
        Notify.create({
          type: 'negative',
          message: 'An error occured while fetching the report data',
          caption: error.message
        })

        showingChart.value = false
      } finally {
        loading.value = false
      }
    }

    const semesters = ['Fall', 'Spring', 'Summer']

    const department = useDepartmentSelector()

    return {
      loading,

      series,
      options,
      showingChart,

      form,
      semesters,
      onGenerate,

      department: ref(department)
    }
  }
})

function useDepartmentSelector () {
  const columns = ref([
    {
      name: 'name',
      label: 'Name',
      field: 'Name',
      sortable: true,
      searchable: true
    }
  ])

  const departmentService = new ODataApiService('/api/Department')

  const entity = {
    key: 'Id',
    name: 'Department',
    displayName: (plural = false) => `Department${plural ? 's' : ''}`,
    route: (key = '') => `/departments/${key}`,
    service: departmentService,
    defaultValue () {
      return {
        Id: 0,
        Name: ''
      }
    }
  }

  return {
    columns,
    entity
  }
}
</script>
