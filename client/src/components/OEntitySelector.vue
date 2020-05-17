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

    <q-dialog v-model="isOpen" persistent @hide="isOpen = false">
      <div style="width: 700px; max-width: 80vw;">
        <o-crud-table
          :entity="entity"
          :data="items"
          :columns="columns"
          :pagination="pagination"
          :actions="actionConfig"
          :selection="multiple ? 'multiple' : 'single'"
          :selected.sync="selected"
        ></o-crud-table>

        <div class="row justify-end q-mt-sm">
          <q-btn color="primary" label="Save" @click="isOpen = false" />
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

    const selected = ref([])

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
      const isModifiedOutside = value !== model.value

      // TODO: Add support for multiple items
      if (props.emitKey === true && props.multiple !== true && isModifiedOutside && value !== undefined && value !== null) {
        const item = await props.entity.service.get(value)

        selected.value = [item]
      }
    })

    return {
      isOpen,

      items,
      pagination,
      actionConfig,

      selected
    }
  }
})
</script>
