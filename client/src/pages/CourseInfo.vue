<template>
  <q-page class="flex flex-center" padding>
    <div class="q-my-lg">
      <o-crud-table :entity="entity" :data="items" :columns="columns" :pagination="pagination">
        <template v-slot:form="{ item }">
          <o-entity-selector
            v-model="item.DepartmentId"
            emit-key
            :columns="department.columns"
            :entity="department.entity"
            :display="model => model.Name"
          />
          <q-input v-model="item.Name" label="Name"></q-input>
          <q-input v-model="item.Code" label="Course Code"></q-input>
          <q-input v-model.number="item.Credit" type="number" min="1" label="Credit"></q-input>
        </template>
      </o-crud-table>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive } from '@vue/composition-api'

import OCrudTable from '../components/OCrudTable'
import OEntitySelector from '../components/OEntitySelector'

import { ODataApiService } from '../services/ApiService'

export default defineComponent({
  name: 'CourseInfoPage',
  components: {
    OCrudTable,
    OEntitySelector
  },
  setup (props, context) {
    const items = ref([])
    const columns = ref([
      {
        name: 'code',
        label: 'Course Code',
        field: 'Code',
        sortable: true,
        searchable: true
      },
      {
        name: 'name',
        label: 'Name',
        field: 'Name',
        sortable: true,
        searchable: true
      },
      {
        name: 'credit',
        label: 'Credit',
        field: 'Credit',
        sortable: true,
        searchable: false
      },
      {
        name: 'departmentId',
        label: 'Department Id',
        field: 'DepartmentId',
        sortable: true
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

    const courseInfoService = new ODataApiService('/api/CourseInfo/')

    const entity = {
      key: 'Id',
      name: 'CourseInfo',
      displayName: () => 'Course Information',
      route: (key = '') => `/course_info/${key}`,
      service: courseInfoService,
      defaultValue () {
        return {
          Id: 0,
          Code: '',
          Name: '',
          Credit: 1,
          DepartmentId: null
        }
      }
    }

    const department = useDepartmentSelector()

    return {
      items,
      columns,
      pagination,
      entity,

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
