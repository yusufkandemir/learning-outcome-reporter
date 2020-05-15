<template>
  <div>
    <q-select v-model="selectedModel" readonly use-chips multiple label="Department">
      <template v-slot:append>
        <q-icon name="mdi-selection-search" class="cursor-pointer" @click="isOpen = true" />
      </template>

      <template v-slot:selected>
        <div v-if="selectedModel">
          <q-chip
            v-for="model in selectedModel"
            :key="model.Id"
            color="grey-9"
            text-color="white"
          >{{ model.Name }}</q-chip>
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
    value: [Object, Array],
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
    }
  },
  setup (props, context) {
    const isOpen = ref(false)

    const items = ref([])
    const pagination = reactive({
      page: 1,
      sortBy: 'name',
      descending: false,
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
    const selectedModel = computed(() => {
      return selected.value
    })

    watch(selectedModel, value => {
      context.emit('input', props.multiple ? value : value[0])
    })

    return {
      isOpen,

      items,
      pagination,
      actionConfig,

      selected,
      selectedModel
    }
  }
})
</script>