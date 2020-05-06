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
          <q-select v-model="item.Type" :options="assignmentTypes" label="Type"></q-select>
          <q-input v-model.number="item.Weight" type="number" min="0" max="1" label="Weight"></q-input>
        </template>
      </o-crud-table>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive } from '@vue/composition-api'

import OCrudTable from '../components/OCrudTable'

export default defineComponent({
  name: 'EditCoursePage',
  components: {
    OCrudTable
  },
  setup (props, context) {
    const items = ref([])
    const columns = ref([
      {
        name: 'type',
        label: 'Type',
        field: 'Type',
        sortable: false,
        searchable: false
      },
      {
        name: 'weight',
        label: 'Weight',
        field: 'Weight',
        sortable: false,
        searchable: false
      },
      { name: 'actions', label: 'Actions', align: 'right' }
    ])
    const pagination = reactive({
      page: 1,
      sortBy: 'type',
      descending: true,
      rowsPerPage: context.root.$q.screen.xs ? 12 : 24,
      rowsNumber: 0
    })

    const assignmentTypes = ['Midterm', 'Final', 'Project', 'LabExam', 'Assignment', 'Other']

    const { courseId, courseInfoId } = context.root.$route.params

    const entity = {
      key: 'Id',
      name: 'Assignment',
      displayName: (plural = false) => `Assignment${plural ? 's' : ''}`,
      route: (key = '') => `/course_info/${courseInfoId}/courses/${courseId}/assignments/${key}`,
      apiRoute: (key = '') => `/Course/${courseId}/Assignments/${key}`,
      defaultValue () {
        return {
          Id: 0,
          Type: '',
          Weight: '1'
        }
      }
    }

    const actionConfig = {
      create: {
        icon: 'mdi-clipboard-check'
      }
    }

    return {
      items,
      columns,
      pagination,
      entity,
      actionConfig,
      assignmentTypes
    }
  }
})
</script>
