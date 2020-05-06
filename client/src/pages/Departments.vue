<template>
  <q-page class="flex flex-center" padding>
    <div class="q-my-lg">
      <o-crud-table
        :entity="entity"
        :data="items"
        :columns="columns"
        :pagination="pagination"
        :actions="actionConfig"
      >
        <template v-slot:form="{ item }">
          <q-input v-model="item.Name" label="Name"></q-input>
        </template>
      </o-crud-table>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive } from '@vue/composition-api'

import OCrudTable from '../components/OCrudTable'
import { ODataApiService } from '../services/ApiService'

export default defineComponent({
  name: 'DepartmentsPage',
  components: {
    OCrudTable
  },
  setup (props, context) {
    const items = ref([])
    const columns = ref([
      {
        name: 'name',
        label: 'Name',
        field: 'Name',
        sortable: true,
        searchable: true
      },
      { name: 'actions', label: 'Actions', align: 'right' }
    ])
    const pagination = reactive({
      page: 1,
      sortBy: 'name',
      descending: false,
      rowsPerPage: context.root.$q.screen.xs ? 12 : 24,
      rowsNumber: 0
    })

    const departmentService = new ODataApiService('/api/Department')

    const entity = {
      key: 'Id',
      name: 'Department',
      displayName: (plural = false) => `Department${plural ? 's' : ''}`,
      route: (key = '') => `/departments/${key}`,
      apiRoute: (key = '') => `Department/${key}`,
      service: departmentService,
      defaultValue () {
        return {
          Id: 0,
          Name: ''
        }
      }
    }

    const actionConfig = {
      create: {
        icon: 'mdi-domain-plus'
      }
    }

    return {
      items,
      columns,
      pagination,
      entity,
      actionConfig
    }
  }
})
</script>
