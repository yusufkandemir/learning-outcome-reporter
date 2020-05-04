import axios from 'axios'

export async function fetchDataFromServer (entity, { startRow, count, search, sortBy, descending }) {
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

  const url = `/api/${entity}?${params.toString()}`

  const response = await axios(url)

  return response.data
}

export async function pushDataToServer (entity, data, isUpdating) {
  return axios({
    url: `/api/${entity}/${isUpdating ? data.Id : ''}`,
    method: isUpdating ? 'PUT' : 'POST',
    data
  })
}
