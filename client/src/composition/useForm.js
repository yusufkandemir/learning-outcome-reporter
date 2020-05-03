import { ref, reactive } from '@vue/composition-api'

export function useForm (defaultValue, onSave) {
  const loading = ref(false)
  const isUpdating = ref(false)
  const isOpen = ref(false)

  const value = reactive({ ...defaultValue })

  const closeForm = () => {
    isOpen.value = false
  }

  const resetForm = () => {
    isUpdating.value = false
    Object.assign(value, defaultValue)
  }

  const editItem = item => {
    isUpdating.value = true
    Object.assign(value, {}, item)
    isOpen.value = true
  }

  const saveForm = async () => {
    loading.value = true

    await onSave(value, isUpdating.value)

    loading.value = false
    closeForm()
    resetForm()
  }

  return {
    loading,
    isUpdating,
    isOpen,
    value,
    saveForm,
    closeForm,
    resetForm,
    editItem
  }
}
