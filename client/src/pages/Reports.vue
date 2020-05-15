<template>
  <div>
    <apexchart width="500" type="bar" :options="options" :series="series"></apexchart>
  </div>
</template>

<script>
import { defineComponent, ref, onMounted } from '@vue/composition-api'
import axios from 'axios'

export default defineComponent({
  name: 'ReportPage',
  setup (props, context) {
    const data = ref([])
    const series = ref([{
      name: 'Avg. Performance',
      data: data.value
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
      const { data } = await axios.get('/api/Reports/Department?DepartmentId=1&Semester=Spring&Year=2019')

      data.value = data.results
    })

    return {
      series,
      options
    }
  }
})
</script>
