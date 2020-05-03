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
            @click="dialog = true"
          />
        </template>

        <template v-slot:body-cell-actions="props">
          <q-td :props="props">
            <q-btn dense round flat color="grey" @click="editItem(props)" icon="mdi-playlist-edit">
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
      <q-dialog v-model="dialog" @hide="resetForm">
        <q-card class="q-pa-sm">
          <q-card-section>
            <span class="text-h5">{{ isUpdating ? 'Edit' : 'Create' }} Department</span>
          </q-card-section>

          <q-card-section>
            <q-input v-model="editedItem.Name" label="Name"></q-input>
          </q-card-section>

          <q-card-actions class="justify-end">
            <q-btn flat color="primary" :loading="formLoading" @click="closeForm">Cancel</q-btn>
            <q-btn flat color="primary" :loading="formLoading" @click="saveForm">Save</q-btn>
          </q-card-actions>
        </q-card>
      </q-dialog>
    </div>
  </q-page>
</template>

<script>
import axios from 'axios'
import { ref, reactive, computed, onMounted, toRefs } from '@vue/composition-api'

async function fetchDataFromServer ({ startRow, count, search, sortBy, descending }) {
  const params = new URLSearchParams({
    $skip: startRow,
    $top: count,
    $count: true
  })

  if (sortBy) {
    params.append('$orderBy', `${sortBy} ${descending ? 'desc' : 'asc'}`)
  }

  if (search?.fields?.length > 0 && search?.term) {
    const filterQuery = search.fields
      .map(field => `contains(${field}, '${search.term}')`)
      .join(' or ')

    params.append('$filter', filterQuery)
  }

  const url = `/api/Department?${params.toString()}`

  const response = await axios(url)

  return response.data
}

async function pushDataToServer (data, isUpdating) {
  return axios({
    url: `/api/Department/${isUpdating ? data.Id : ''}`,
    method: isUpdating ? 'PUT' : 'POST',
    data
  })
}

export default {
  name: 'DepartmentsPage',
  setup (props, context) {
    const formLoading = ref(false)
    const isUpdating = ref(false)
    const dialog = ref(false)

    const defaultItem = {
      Name: ''
    }
    const editedItem = reactive({ ...defaultItem })

    const closeForm = () => {
      dialog.value = false
    }

    const resetForm = () => {
      isUpdating.value = false
      Object.assign(editedItem, defaultItem)
    }

    const editItem = ({ row: item }) => {
      isUpdating.value = true
      Object.assign(editedItem, {}, item)
      dialog.value = true
    }

    const saveForm = async () => {
      formLoading.value = true
      await pushDataToServer(editedItem, isUpdating.value)

      formLoading.value = false
      closeForm()
      resetForm()

      onRequest({
        pagination,
        filter: filter.value
      })
    }

    const filter = ref('')
    const loading = ref(false)
    const items = ref([])
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

      // Trigger table for update
      onRequest({
        pagination,
        filter: filter.value
      })
    }

    onMounted(() => {
      onRequest({
        pagination,
        filter: filter.value
      })
    })

    const onRequest = async ({ pagination: newPagination, filter: newFilter }) => {
      loading.value = true

      const { page, rowsPerPage, rowsNumber, sortBy, descending } = newPagination

      // Get all rows if 'All' (0) is selected
      const fetchCount = rowsPerPage === 0 ? rowsNumber : rowsPerPage

      // Calculate starting row of data
      const startRow = (page - 1) * rowsPerPage

      const search = {
        term: newFilter,
        fields: searchableFields.value
      }

      let data
      try {
        data = await fetchDataFromServer({ startRow, count: fetchCount, search, sortBy, descending })
      } catch (error) {
        loading.value = false
        context.root.$q.notify({
          type: 'negative',
          message: 'An error occured while fetching data from the server',
          caption: error.message
        })

        return
      }

      // Clear out existing data and add new
      items.value.splice(0, items.value.length, ...data.value)

      // Update rowsNumber with total count
      pagination.rowsNumber = parseInt(data['@odata.count'])

      // Update the local pagination object
      pagination.page = page
      pagination.rowsPerPage = rowsPerPage
      pagination.sortBy = sortBy
      pagination.descending = descending

      loading.value = false
    }

    return {
      // Form (create/edit) related
      formLoading,
      isUpdating,
      dialog,
      editedItem,
      saveForm,
      closeForm,
      resetForm,
      editItem,
      // Delete related
      deleteItem,
      // Table related
      filter,
      loading,
      items,
      rowsPerPageOptions,
      pagination: toRefs(pagination),
      columns,
      searchableFields,
      onRequest
    }
  }
}
</script>
