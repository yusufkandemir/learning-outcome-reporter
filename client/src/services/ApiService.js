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

  async getAll ({ startRow, count, search, sortBy, descending, parameters }) {
    const params = new URLSearchParams({
      ...parameters,
      $count: true
    })

    if (startRow !== undefined) {
      params.set('$skip', startRow)
    }

    if (count !== undefined) {
      params.set('$top', count)
    }

    if (sortBy) {
      params.set('$orderBy', `${sortBy} ${descending ? 'desc' : 'asc'}`)
    }

    if (search?.fields?.length > 0 && search?.term) {
      const filterQuery = search.fields
        .map(field => `contains(${field}, '${search.term}')`)
        .join(' or ')

      params.append('$filter', filterQuery)
    }

    const { data } = await this.client.get(`?${params.toString()}`)

    return {
      count: data['@odata.count'],
      items: data.value
    }
  }

  async get (key) {
    const response = await this.client.get(`/${key}`)

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
