<template>
  <q-page class="flex row justify-center content-start items-center q-col-gutter-lg" padding>
    <div class="row justify-center col-12">
      <div class="col-12 col-md-8 col-lg-4">
        <q-card class="q-pa-sm">
          <q-card-section>
            <span class="text-h5">Department Report</span>
          </q-card-section>

          <q-card-section>
            <q-input
              v-model.number="form.departmentId"
              label="Deparment Id"
              type="number"
              min="1"
              :loading="loading"
            ></q-input>
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

export default defineComponent({
  name: 'ReportPage',
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
      departmentId: null,
      semester: null,
      year: null
    })

    const onGenerate = async () => {
      loading.value = true

      try {
        const params = new URLSearchParams({
          DepartmentId: form.departmentId,
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

    return {
      loading,

      series,
      options,
      showingChart,

      form,
      semesters,
      onGenerate
    }
  }
})
</script>
