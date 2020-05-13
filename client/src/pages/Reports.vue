<template>
  <div>
    <apexchart width="500" type="bar" :options="options" :series="series"></apexchart>
  </div>
</template>

<script>
import { defineComponent, ref } from '@vue/composition-api'

export default defineComponent({
  name: 'ReportPage',
  setup (props, context) {
    const series = ref([{
      name: 'Avg. Performance',
      data: [{
        x: '1',
        y: 44
      }, {
        x: '2',
        y: 51
      },
      {
        x: '3',
        y: 51
      },
      {
        x: '4',
        y: 51
      },
      {
        x: '5',
        y: 51
      }]
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

    return {
      series,
      options
    }
  }
})
</script>
