<script setup lang="ts">
import { ref } from 'vue'
import { Dialog, InputText, InputNumber, Button } from 'primevue'

defineProps<{
  visible: boolean
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  save: [name: string, count: number]
}>()

const itemName = ref('')
const itemCount = ref(1)

function onSubmit() {
  emit('save', itemName.value, itemCount.value)
  itemName.value = ''
  itemCount.value = 1
}
</script>

<template>
  <Dialog
    :visible="visible"
    @update:visible="$emit('update:visible', $event)"
    modal
    header="Add Item"
  >
    <form @submit.prevent="onSubmit" class="dialog-form">
      <label for="itemName">Item Name:</label>
      <InputText
        id="itemName"
        v-model="itemName"
        autocomplete="off"
        placeholder="Enter item name"
        class="w-full"
      />

      <label for="itemCount">Count:</label>
      <InputNumber
        id="itemCount"
        v-model="itemCount"
        showButtons
        :min="0"
        placeholder="Enter quantity"
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
