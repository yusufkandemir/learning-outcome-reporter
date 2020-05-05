<template>
  <q-page class="flex flex-center" padding>
    <o-crud-table
      class="q-my-lg col-md-12 col-lg-8"
      :entity="entity"
      :data="items"
      :columns="columns"
      :pagination="pagination"
      :actions="actionConfig"
    >
      <template v-slot:form="{ item }">
        <q-input v-model="item.Code" label="Code"></q-input>
        <q-input v-model="item.Description" label="Description" autogrow></q-input>
      </template>
    </o-crud-table>
  </q-page>
</template>

<script>
import { ref, reactive } from '@vue/composition-api'

import OCrudTable from '../components/OCrudTable'

export default {
  name: 'EditDepartmentPage',
  components: {
    OCrudTable
  },
  setup (props, context) {
    const items = ref([])
    const columns = ref([
      {
        name: 'code',
        label: 'Code',
        field: 'Code',
        sortable: true,
        searchable: true
      },
      {
        name: 'description',
        label: 'Description',
        field: 'Description',
        sortable: true,
        searchable: true
      },
      { name: 'actions', label: 'Actions', align: 'right' }
    ])
    const pagination = reactive({
      page: 1,
      sortBy: 'code',
      descending: false,
      rowsPerPage: context.root.$q.screen.xs ? 12 : 24,
      rowsNumber: 0
    })

    const departmentId = context.root.$route.params.id

    const entity = {
      key: 'Id',
      name: 'ProgramOutcome',
      displayName: (plural = false) => `Program Outcome${plural ? 's' : ''}`,
      route: (key = '') => `/departments/${departmentId}/outcomes/${key}`,
      apiRoute: (key = '') => `Department/${departmentId}/Outcomes/${key}`,
      defaultValue () {
        return {
          Id: 0,
          Code: '',
          Description: ''
        }
      }
    }

    const actionConfig = {
      create: {
        icon: 'mdi-plus'
      },
      edit: {
        enabled: false
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
}
</script>
