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
  data () {
    return {
      loading: false,
      formLoading: false,
      filter: '',
      items: [],
      dialog: false,
      isUpdating: false,
      editedItem: {
        Name: ''
      },
      defaultItem: {
        Name: ''
      },
      columns: [
        {
          name: 'name',
          label: 'Name',
          field: 'Name',
          sortable: true,
          searchable: true
        },
        { name: 'actions', label: 'Actions', align: 'right' }
      ],
      pagination: {
        page: 1,
        sortBy: 'name',
        descending: false,
        rowsPerPage: this.$q.screen.xs ? 12 : 24,
        rowsNumber: 0
      },
      rowsPerPageOptions: [12, 24, 36, 48]
    }
  },
  computed: {
    searchableFields () {
      return this.columns
        .filter(column => column.searchable)
        .map(column => column.field)
    }
  },
  mounted () {
    this.onRequest({
      pagination: this.pagination,
      filter: ''
    })
  },
  methods: {
    async onRequest ({ pagination, filter }) {
      this.loading = true

      const { page, rowsPerPage, rowsNumber, sortBy, descending } = pagination

      // Get all rows if 'All' (0) is selected
      const fetchCount = rowsPerPage === 0 ? rowsNumber : rowsPerPage

      // Calculate starting row of data
      const startRow = (page - 1) * rowsPerPage

      const search = {
        term: filter,
        fields: this.searchableFields
      }

      let data
      try {
        data = await fetchDataFromServer({ startRow, count: fetchCount, search, sortBy, descending })
      } catch (error) {
        this.loading = false
        this.$q.notify({
          type: 'negative',
          message: 'An error occured while fetching data from the server',
          caption: error.message
        })

        return
      }

      // Clear out existing data and add new
      this.items.splice(0, this.items.length, ...data.value)

      // Update rowsNumber with total count
      this.pagination.rowsNumber = parseInt(data['@odata.count'])

      // Update the local pagination object
      this.pagination.page = page
      this.pagination.rowsPerPage = rowsPerPage
      this.pagination.sortBy = sortBy
      this.pagination.descending = descending

      this.loading = false
    },

    async saveForm () {
      this.formLoading = true
      await pushDataToServer(this.editedItem, this.isUpdating)

      this.formLoading = false
      this.closeForm()
      this.resetForm()

      // Trigger table for update
      this.onRequest({
        pagination: this.pagination,
        filter: this.filter
      })
    },

    closeForm () {
      this.dialog = false
    },

    resetForm () {
      this.isUpdating = false
      this.editedItem = Object.assign({}, this.defaultItem)
    },

    async editItem ({ row: item }) {
      this.isUpdating = true
      this.editedItem = Object.assign({}, item)
      this.dialog = true
    },

    async deleteItem ({ row: item }) {
      if (!confirm('Are you sure you want to delete this item?')) return

      this.loading = true
      await axios.delete(`/api/Department/${item.Id}`)
      this.loading = false

      // Trigger table for update
      this.onRequest({
        pagination: this.pagination,
        filter: this.filter
      })
    }
  }
}
</script>
