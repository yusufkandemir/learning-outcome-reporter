<template>
  <q-page class="flex flex-center" padding>
    <div class="q-my-lg">
      <q-table
        title="Deparments"
        row-key="Id"
        binary-state-sort
        :data="items"
        :columns="columns"
        :pagination.sync="pagination"
        :rows-per-page-options="rowsPerPageOptions"
        :loading="loading"
        :filter="filter"
        @request="onRequest"
      >
        <template v-slot:top-right>
          <q-input dense debounce="500" v-model="filter" placeholder="Search">
            <template v-slot:append>
              <q-icon name="mdi-magnify" />
            </template>
          </q-input>

          <q-btn
            class="q-ml-md"
            color="primary"
            icon="mdi-domain-plus"
            label="New Department"
            @click="isFormOpen = true"
          />
        </template>

        <template v-slot:body-cell-actions="props">
          <q-td :props="props">
            <q-btn
              dense
              round
              flat
              color="grey"
              @click="$refs.form.editItem(props.row)"
              icon="mdi-playlist-edit"
            >
              <q-tooltip>Quick Edit</q-tooltip>
            </q-btn>
            <q-btn
              dense
              round
              flat
              color="grey"
              :to="`/departments/${props.row.Id}`"
              icon="mdi-pencil"
            >
              <q-tooltip>Edit</q-tooltip>
            </q-btn>
            <q-btn dense round flat color="grey" @click="deleteItem(props)" icon="mdi-delete">
              <q-tooltip>Delete</q-tooltip>
            </q-btn>
          </q-td>
        </template>
      </q-table>

      <o-popup-form
        ref="form"
        v-model="isFormOpen"
        title="Department"
        :default-value="defaultValue"
        @save="onSave"
        v-slot="{ value }"
      >
        <q-input v-model="value.Name" label="Name"></q-input>
      </o-popup-form>
    </div>
  </q-page>
</template>

<script>
import axios from 'axios'
import { ref, reactive, computed, onMounted, toRefs } from '@vue/composition-api'

import OPopupForm from '../components/OPopupForm'
import { useServerSideProcessedTable } from '../composition/useServerSideProcessedTable'

import { fetchDataFromServer, pushDataToServer } from '../services/ApiService'

export default {
  name: 'DepartmentsPage',
  components: {
    OPopupForm
  },
  setup (props, context) {
    const defaultValue = {
      Id: 0,
      Name: ''
    }
    const isFormOpen = ref(false)

    const onSave = async (data, isUpdating) => {
      await pushDataToServer(data, isUpdating)

      refreshTable()
    }

    const filter = ref('')
    const rowsPerPageOptions = [12, 24, 36, 48]
    const pagination = reactive({
      page: 1,
      sortBy: 'name',
      descending: false,
      rowsPerPage: context.root.$q.screen.xs ? 12 : 24,
      rowsNumber: 0
    })
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

    const searchableFields = computed(() => {
      return columns.value
        .filter(column => column.searchable)
        .map(column => column.field)
    })

    const deleteItem = async ({ row: item }) => {
      if (!confirm('Are you sure you want to delete this item?')) return

      loading.value = true
      await axios.delete(`/api/Department/${item.Id}`)
      loading.value = false

      refreshTable()
    }

    onMounted(() => {
      refreshTable()
    })

    const { items, loading, onRequest } = useServerSideProcessedTable(
      pagination,
      async ({ startRow, count, filter, sortBy, descending }) => {
        const search = {
          term: filter,
          fields: searchableFields.value
        }

        try {
          const data = await fetchDataFromServer({ startRow, count, search, sortBy, descending })

          return {
            items: data.value,
            count: data['@odata.count']
          }
        } catch (error) {
          context.root.$q.notify({
            type: 'negative',
            message: 'An error occured while fetching data from the server',
            caption: error.message
          })

          throw error
        }
      }
    )

    const refreshTable = () => {
      onRequest({
        pagination,
        filter: filter.value
      })
    }

    return {
      // Form (create/edit) related
      onSave,
      isFormOpen,
      defaultValue,
      // Delete related
      deleteItem,
      // Table related
      filter,
      loading,
      items,
      rowsPerPageOptions,
      pagination: toRefs(pagination),
      columns,
      onRequest
    }
  }
}
</script>
