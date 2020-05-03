import { ref } from '@vue/composition-api'

export function useServerSideProcessedTable (pagination, fetchData) {
  const loading = ref(false)
  const items = ref([])

  const onRequest = async ({ pagination: newPagination, filter }) => {
    loading.value = true

    const { page, rowsPerPage, rowsNumber, sortBy, descending } = newPagination

    // Get all rows if 'All' (0) is selected
    const fetchCount = rowsPerPage === 0 ? rowsNumber : rowsPerPage

    // Calculate starting row of data
    const startRow = (page - 1) * rowsPerPage

    let data
    try {
      data = await fetchData({ startRow, count: fetchCount, filter, sortBy, descending })
    } catch (error) {
      loading.value = false

      return
    }

    // Clear out existing data and add new
    items.value.splice(0, items.value.length, ...data.items)

    // Update rowsNumber with total count
    pagination.rowsNumber = parseInt(data.count)

    // Update the local pagination object
    pagination.page = page
    pagination.rowsPerPage = rowsPerPage
    pagination.sortBy = sortBy
    pagination.descending = descending

    loading.value = false
  }

  return {
    items,
    loading,
    onRequest
  }
}
