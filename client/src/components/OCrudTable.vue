<template>
  <div class="q-my-lg">
    <q-table
      :title="entity.displayName(true)"
      :row-key="entity.key"
      binary-state-sort
      :rows-per-page-options="rowsPerPageOptions"
      :loading="loading"
      :filter="filter"
      @request="onRequest"
      v-bind="$attrs"
      v-on="$listeners"
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
          :label="`New ${entity.displayName()}`"
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
          <q-btn dense round flat color="grey" :to="entity.route(props.row.Id)" icon="mdi-pencil">
            <q-tooltip>Edit</q-tooltip>
          </q-btn>
          <q-btn dense round flat color="grey" @click="deleteItem(props)" icon="mdi-delete">
            <q-tooltip>Delete</q-tooltip>
          </q-btn>
        </q-td>
      </template>

      <template v-for="(_, slot) of $scopedSlots" v-slot:[slot]="scope">
        <slot :name="slot" v-bind="scope" />
      </template>
    </q-table>

    <o-popup-form
      ref="form"
      v-model="isFormOpen"
      :title="entity.displayName()"
      :default-value="defaultValue"
      @save="onSave"
      v-slot:default="{ value }"
    >
      <slot name="form" :item="value" />
    </o-popup-form>
  </div>
</template>

<script>
import axios from 'axios'
import { ref, computed, onMounted, watch } from '@vue/composition-api'

import OPopupForm from '../components/OPopupForm'
import { useServerSideProcessedTable } from '../composition/useServerSideProcessedTable'

import { fetchDataFromServer, pushDataToServer } from '../services/ApiService'

export default {
  name: 'OCrudTable',
  components: {
    OPopupForm
  },
  props: {
    entity: {
      type: Object,
      required: true
    }
  },
  inheritAttrs: false,
  setup (props, context) {
    const defaultValue = {
      Id: 0,
      Name: ''
    }
    const isFormOpen = ref(false)

    const onSave = async (data, isUpdating) => {
      await pushDataToServer(props.entity.name, data, isUpdating)

      refreshTable()
    }

    const filter = ref('')
    const rowsPerPageOptions = [12, 24, 36, 48]

    const searchableFields = computed(() => {
      return context.attrs.columns
        .filter(column => column.searchable)
        .map(column => column.field)
    })

    const deleteItem = async ({ row: item }) => {
      if (!confirm('Are you sure you want to delete this item?')) return

      loading.value = true
      await axios.delete(`/api/${props.entity.name}/${item.Id}`)
      loading.value = false

      refreshTable()
    }

    onMounted(() => {
      refreshTable()
    })

    const { items, loading, onRequest } = useServerSideProcessedTable(
      context.attrs.pagination,
      async ({ startRow, count, filter, sortBy, descending }) => {
        const search = {
          term: filter,
          fields: searchableFields.value
        }

        try {
          const data = await fetchDataFromServer(props.entity.name, { startRow, count, search, sortBy, descending })

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
        pagination: context.attrs.pagination,
        filter: filter.value
      })
    }

    watch(items, value => {
      const data = context.attrs.data
      data.splice(0, data.length, ...value)
    })

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
      onRequest
    }
  }
}
</script>
