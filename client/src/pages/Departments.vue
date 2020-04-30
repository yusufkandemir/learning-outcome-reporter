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
        </template>
      </q-table>
    </div>
  </q-page>
</template>

<script>
import axios from 'axios'

export default {
  name: 'DepartmentsPage',
  data () {
    return {
      loading: false,
      filter: '',
      items: [],
      columns: [
        {
          name: 'name',
          label: 'Name',
          field: 'Name',
          sortable: true,
          searchable: true
        }
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
        fields: this.columns
          .filter(column => column.searchable)
          .map(column => column.field)
      }

      let data
      try {
        data = await this.fetchDataFromServer({ startRow, count: fetchCount, search, sortBy, descending })
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

    async fetchDataFromServer ({ startRow, count, search, sortBy, descending }) {
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
  }
}
</script>
