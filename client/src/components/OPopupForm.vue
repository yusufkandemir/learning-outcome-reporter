<template>
  <q-dialog v-model="isOpen" @hide="resetForm">
    <q-card class="q-pa-sm">
      <q-card-section>
        <span class="text-h5">{{ isUpdating ? 'Edit' : 'Create' }} {{ title }}</span>
      </q-card-section>

      <q-card-section>
        <slot :value="formValue" />
      </q-card-section>

      <q-card-actions class="justify-end">
        <q-btn flat color="primary" :loading="loading" @click="closeForm">Cancel</q-btn>
        <q-btn flat color="primary" :loading="loading" @click="saveForm">Save</q-btn>
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script>
import { watch } from '@vue/composition-api'
import { useForm } from '../composition/useForm'

export default {
  name: 'OPopupForm',
  props: {
    value: Boolean,
    title: String,
    defaultValue: {
      type: Object,
      required: true
    }
  },
  setup (props, context) {
    const { isOpen, isUpdating, loading, saveForm, resetForm, closeForm, value: formValue, editItem } = useForm(
      props.defaultValue,
      async (data, isUpdating) => {
        context.emit('save', data, isUpdating)
      }
    )

    // 2-way data binding of value prop and isOpen
    watch(() => props.value, value => {
      isOpen.value = value
    })
    watch(isOpen, value => {
      context.emit('input', value)
    })

    return {
      isOpen,
      isUpdating,
      loading,
      saveForm,
      resetForm,
      closeForm,
      formValue,
      editItem
    }
  }
}
</script>
