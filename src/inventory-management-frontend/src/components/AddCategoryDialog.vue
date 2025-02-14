<script setup lang="ts">
import { ref } from 'vue'
import { Dialog, InputText, Button } from 'primevue'

defineProps<{
  visible: boolean
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  save: [name: string]
}>()

const categoryName = ref('')

function onSubmit() {
  emit('save', categoryName.value)
  categoryName.value = ''
}
</script>

<template>
  <Dialog
    :visible="visible"
    @update:visible="$emit('update:visible', $event)"
    modal
    header="Add Category"
  >
    <form @submit.prevent="onSubmit" class="dialog-form">
      <label for="categoryName">Category Name:</label>
      <InputText
        id="categoryName"
        v-model="categoryName"
        autocomplete="off"
        placeholder="Enter category name"
        class="w-full"
      />
      <Button type="submit" label="Save" class="w-full" />
    </form>
  </Dialog>
</template>

<style scoped>
.dialog-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1rem;
}

.dialog-form label {
  font-weight: 600;
  margin-bottom: -0.5rem;
}

.w-full {
  width: 100%;
}
</style>
