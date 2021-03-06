<template>
  <div>
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
        <q-input
          v-if="actionConfig.search.enabled"
          v-model="filter"
          dense
          debounce="500"
          :placeholder="actionConfig.search.label"
        >
          <template v-slot:append>
            <q-icon :name="actionConfig.search.icon" />
          </template>
        </q-input>

        <q-btn
          v-if="actionConfig.create.enabled"
          class="q-ml-md"
          color="primary"
          :icon="actionConfig.create.icon"
          :label="actionConfig.create.label"
          @click="isFormOpen = true"
        />
      </template>

      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn
            v-if="actionConfig.quickEdit.enabled"
            dense
            round
            flat
            color="grey"
            @click="$refs.form.editItem(props.row)"
            :icon="actionConfig.quickEdit.icon"
          >
            <q-tooltip>{{ actionConfig.quickEdit.label }}</q-tooltip>
          </q-btn>
          <q-btn
            v-if="actionConfig.edit.enabled"
            dense
            round
            flat
            color="grey"
            :to="entity.route(props.row.Id)"
            :icon="actionConfig.edit.icon"
          >
            <q-tooltip>{{ actionConfig.edit.label }}</q-tooltip>
          </q-btn>
          <q-btn
            v-if="actionConfig.delete.enabled"
            dense
            round
            flat
            color="grey"
            @click="deleteItem(props.row)"
            :icon="actionConfig.delete.icon"
          >
            <q-tooltip>{{ actionConfig.delete.label }}</q-tooltip>
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
import { extend } from 'quasar'
import { defineComponent, ref, computed, onMounted, watch } from '@vue/composition-api'

import OPopupForm from '../components/OPopupForm'
import { useServerSideProcessedTable } from '../composition/useServerSideProcessedTable'

export default defineComponent({
  name: 'OCrudTable',
  components: {
    OPopupForm
  },
  props: {
    entity: {
      type: Object,
      required: true
    },
    actions: Object
  },
  inheritAttrs: false,
  setup (props, context) {
    const { items, loading, filter, onRequest, refreshTable, rowsPerPageOptions } = useTable(props, context)

    const { actionConfig } = useAction(props)

    const isFormOpen = ref(false)

    const onSave = async (data, isUpdating) => {
      if (!isUpdating) {
        await props.entity.service.create(data)
      } else {
        await props.entity.service.update(data[props.entity.key], data)
      }

      refreshTable()
    }

    const deleteItem = async item => {
      if (!confirm('Are you sure you want to delete this item?')) return

      loading.value = true
      await props.entity.service.delete(item[props.entity.key])
      loading.value = false

      refreshTable()
    }

    onMounted(() => {
      refreshTable()
    })

    return {
      // Form (create/edit) related
      onSave,
      isFormOpen,
      defaultValue: props.entity.defaultValue(),
      // Delete related
      deleteItem,
      // Table related
      items,
      filter,
      loading,
      onRequest,
      rowsPerPageOptions,
      // Other
      actionConfig
    }
  }
})

function useAction (props) {
  const actionsDefaults = {
    create: {
      enabled: true,
      icon: 'mdi-plus',
      label: `New ${props.entity.displayName()}`
    },
    quickEdit: {
      enabled: true,
      icon: 'mdi-playlist-edit',
      label: 'Quick Edit'
    },
    edit: {
      enabled: true,
      icon: 'mdi-pencil',
      label: 'Edit'
    },
    delete: {
      enabled: true,
      icon: 'mdi-delete',
      label: 'Delete'
    },
    search: {
      enabled: true,
      icon: 'mdi-magnify',
      label: 'Search'
    }
  }

  const actionConfig = computed(() => extend(true, actionsDefaults, props.actions))

  return {
    actionConfig
  }
}

function useTable (props, context) {
  const filter = ref('')
  const rowsPerPageOptions = [12, 24, 36, 48]

  const searchableFields = computed(() => {
    return context.attrs.columns
      .filter(column => column.searchable)
      .map(column => column.field)
  })

  const { items, loading, onRequest } = useServerSideProcessedTable(
    context.attrs.pagination,
    async ({ startRow, count, filter, sortBy, descending }) => {
      const search = {
        term: filter,
        fields: searchableFields.value
      }

      try {
        const data = await props.entity.service.getAll({ startRow, count, search, sortBy, descending })

        return {
          items: data.items,
          count: data.count
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

  watch(items, value => {
    const data = context.attrs.data
    data.splice(0, data.length, ...value)
  })

  const refreshTable = () => {
    onRequest({
      pagination: context.attrs.pagination,
      filter: filter.value
    })
  }

  return {
    filter,
    loading,
    items,
    onRequest,
    refreshTable,
    rowsPerPageOptions
  }
}
</script>
