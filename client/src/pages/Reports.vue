<template>
  <q-page class="flex row justify-center content-start items-center q-col-gutter-lg" padding>
    <div class="row justify-center col-12">
      <div class="col-6">
        <apexchart type="bar" :options="options" :series="series"></apexchart>
        <q-inner-loading :showing="loading">
          <q-spinner-gears size="xl" color="primary" />
        </q-inner-loading>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, onMounted } from '@vue/composition-api'
import axios from 'axios'
import { Notify } from 'quasar'

export default defineComponent({
  name: 'ReportPage',
  setup (props, context) {
    const loading = ref(false)

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

    onMounted(async () => {
      loading.value = true

      try {
        const response = await axios.get('/api/Reports/Department?DepartmentId=1&Semester=Fall&Year=2020')

        series.value = [{
          name: 'Avg. Performance',
          data: response.data.results
        }]
      } catch (error) {
        Notify.create({
          type: 'negative',
          message: 'An error occured while fetching the report data',
          caption: error.message
        })
      } finally {
        loading.value = false
      }
    })

    return {
      loading,

      series,
      options
    }
  }
})
</script>
