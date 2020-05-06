import axios from 'axios'

const getClient = (baseURL = '') => {
  const options = {
    baseURL
  }

  const client = axios.create(options)

  return client
}

export class ODataApiService {
  constructor (baseURL = '') {
    this.client = getClient(baseURL)
  }

  async getAll ({ startRow, count, search, sortBy, descending }) {
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

    const response = await this.client.get(`?${params.toString()}`)

    return response.data
  }

  async create (data) {
    const response = await this.client.post('/', data)

    return response.data
  }

  async update (key, data) {
    const response = await this.client.put(`/${key}`, data)

    return response.data
  }

  async delete (key) {
    const response = await this.client.delete(`/${key}`)

    return response.data
  }
}

export async function fetchDataFromServer (path, { startRow, count, search, sortBy, descending }) {
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

  const url = `/api/${path}?${params.toString()}`

  const response = await axios(url)

  return response.data
}

export async function pushDataToServer (path, data, isUpdating) {
  return axios({
    url: `/api/${path}`,
    method: isUpdating ? 'PUT' : 'POST',
    data
  })
}
