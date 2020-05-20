<template>
  <div>
    <q-select
      v-model="selected"
      readonly
      use-chips
      multiple
      hide-dropdown-icon
      :label="entity.displayName(multiple)"
    >
      <template v-slot:append>
        <q-icon name="mdi-selection-search" class="cursor-pointer" @click="isOpen = true" />
      </template>

      <template v-slot:selected>
        <div v-if="selected">
          <q-chip
            v-for="model in selected"
            :key="model[entity.Key]"
            color="grey-9"
            text-color="white"
          >{{ display(model) }}</q-chip>
        </div>
      </template>
    </q-select>

    <q-dialog v-model="isOpen" :persistent="liveEdit !== true" @hide="isOpen = false">
      <div style="width: 700px; max-width: 80vw;">
        <o-crud-table
          :entity="entity"
          :data="items"
          :columns="columns"
          :pagination="pagination"
          :actions="actionConfig"
          :selection="multiple ? 'multiple' : 'single'"
          :selected.sync="internalSelected"
          @selection="onSelection"
        ></o-crud-table>

        <div v-if="liveEdit !== true" class="row justify-end q-gutter-xs q-mt-xs">
          <q-btn label="Cancel" @click="onCancel" />
          <q-btn color="primary" label="Save" @click="onSave" />
        </div>
      </div>
    </q-dialog>
  </div>
</template>

<script>
import { defineComponent, ref, reactive, computed, watch } from '@vue/composition-api'

import OCrudTable from '../components/OCrudTable'

export default defineComponent({
  name: 'OEntitySelector',
  components: {
    OCrudTable
  },
  props: {
    value: {
      required: true
    },
    columns: {
      required: true,
      type: Array
    },
    entity: {
      required: true,
      type: Object
    },
    multiple: {
      type: Boolean,
      default: false
    },
    display: {
      type: Function,
      default (model) {
        return model[this.entity.key]
      }
    },
    emitKey: Boolean,
    liveEdit: Boolean,
    sortBy: {
      type: Object,
      default () {
        return {
          // Find first sortable column
          field: this.columns.find(column => column.sortable).name,
          descending: false
        }
      }
    }
  },
  setup (props, context) {
    const isOpen = ref(false)

    const items = ref([])
    const pagination = reactive({
      page: 1,
      sortBy: props.sortBy.field,
      descending: props.sortBy.descending,
      rowsPerPage: context.root.$q.screen.xs ? 12 : 24,
      rowsNumber: 0
    })

    const actionConfig = {
      create: {
        enabled: false
      },
      quickEdit: {
        enabled: false
      },
      edit: {
        enabled: false
      },
      delete: {
        enabled: false
      }
    }

    // TODO: Simplify the internal logic
    const selected = ref([])
    const internalSelected = ref([])

    watch(selected, value => {
      internalSelected.value = value
    })

    watch(internalSelected, value => {
      if (props.liveEdit === true) {
        selected.value = value
      }
    })

    const model = computed(() => {
      let values = selected.value

      if (props.emitKey === true) {
        values = values.map(value => value[props.entity.key])
      }

      return props.multiple === true ? values : (values[0] !== undefined ? values[0] : null)
    })

    watch(model, value => {
      context.emit('input', value)
    })

    watch(() => props.value, async (value, oldValue) => {
      const isModifiedOutside = props.multiple !== true ? value !== model.value : (isArraysEqual(value, model.value) !== true)

      if (props.emitKey === true && isModifiedOutside === true && value !== undefined && value !== null) {
        if (props.multiple !== true) {
          const item = await props.entity.service.get(value)

          selected.value = [item]
        } else {
          if (value.length <= 0) return

          const { items } = await props.entity.service.getAll({ parameters: { $filter: `${props.entity.key} in (${value.join()})` } })

          selected.value = items
        }
      }

      // TODO: Add support for updating the internal model with the outside value, for emitKey === false
    })

    const onSave = () => {
      selected.value = internalSelected.value
      isOpen.value = false
    }

    const onCancel = () => {
      internalSelected.value = selected.value
      isOpen.value = false
    }

    const onSelection = details => {
      const currentKeys = selected.value.map(item => item[props.entity.key])
      // Eliminate the unchanged keys, because 'details.keys' can contain extra keys when 'toggle all' checkbox in QTable is used
      const changedKeys = details.keys.filter(key => details.added ^ currentKeys.includes(key))

      context.emit('selection', {
        isAdded: details.added,
        keys: changedKeys
      })
    }

    return {
      isOpen,
      onSave,
      onCancel,

      items,
      pagination,
      actionConfig,

      selected,
      internalSelected,
      onSelection
    }
  }
})

const isArraysEqual = (first, second) => (first.length === second.length) && first.every((element, index) => element === second[index])
</script>

<style lang="sass">
</style>
