<script setup lang="ts">
import { onMounted, ref } from 'vue'
import {
  Button,
  InputNumber,
  Tree,
  Popover,
  type TreeExpandedKeys,
  ProgressSpinner,
} from 'primevue'
import type { TreeNode } from 'primevue/treenode'
import type { Category, Item } from '@/types/api'
import {
  getParentNodeForNode,
  itemDtoToTreeNode,
  categoryDtoToTreeNode,
  customSortingForTreeNodes,
} from '@/utils/tree'
import { categoriesApi, itemsApi } from '@/api'
import AddCategoryDialog from '@/components/AddCategoryDialog.vue'
import AddItemDialog from '@/components/AddItemDialog.vue'

const categoryForChildCreation = ref<TreeNode | null>(null)
const addCategoryDialogVisible = ref(false)
const addItemDialogVisible = ref(false)

const expandedKeys = ref<TreeExpandedKeys>({})
const nodes = ref<TreeNode[]>([])
const popover = ref<InstanceType<typeof Popover>>()
const isLoading = ref<boolean>(false)

async function loadRootCategories() {
  try {
    isLoading.value = true

    const categories = await categoriesApi.getUserRootCategories()

    for (const categoryDto of categories) {
      nodes.value.push(categoryDtoToTreeNode(categoryDto))
    }

    nodes.value.sort(customSortingForTreeNodes)
  } catch (err) {
    console.error('Cannot get root categories cause of', err)
  } finally {
    isLoading.value = false
  }
}

async function addCategoryButtonHandler(name: string) {
  if (categoryForChildCreation.value && categoryForChildCreation.value?.type != 'category') {
    return
  }
  const category = categoryForChildCreation.value?.data as Category | null

  try {
    const newCategory = await categoriesApi.create({
      name: name,
      parentId: category ? category.id : null,
    })
    const newNode = categoryDtoToTreeNode(newCategory)

    if (categoryForChildCreation.value == null) {
      nodes.value.push(newNode)
      nodes.value.sort(customSortingForTreeNodes)
    } else {
      categoryForChildCreation.value?.children?.push(newNode)
      categoryForChildCreation.value?.children?.sort(customSortingForTreeNodes)
    }
  } catch (err) {
    console.error('Cannot create category cause of', err)
  }

  categoryForChildCreation.value = null
  addCategoryDialogVisible.value = false
}

async function addItemButtonHandler(name: string, count: number) {
  if (categoryForChildCreation.value?.type != 'category') return
  const category = categoryForChildCreation.value?.data as Category

  try {
    const newItem = await itemsApi.create({
      name: name,
      count: count,
      categoryId: category.id,
    })
    const newNode = itemDtoToTreeNode(newItem)

    categoryForChildCreation.value?.children?.push(newNode)
    categoryForChildCreation.value?.children?.sort(customSortingForTreeNodes)
  } catch (err) {
    console.error('Cannot create item cause of', err)
  }

  categoryForChildCreation.value = null
  addItemDialogVisible.value = false
}

async function removeItemButtonHandler(itemNode: TreeNode) {
  if (itemNode.type != 'item') return
  const item = itemNode.data as Item

  try {
    await itemsApi.remove(item.id)

    const parentNode = getParentNodeForNode(nodes.value, itemNode)
    if (parentNode) {
      parentNode.children = parentNode.children?.filter((i) => i !== itemNode)
    } else {
      nodes.value = nodes.value.filter((i) => i != itemNode)
    }
  } catch (err) {
    console.error('Cannot remove item cause of', err)
  }
}

async function removeCategoryButtonHandler(categoryNode: TreeNode) {
  if (categoryNode.type != 'category') return
  const category = categoryNode.data as Category

  try {
    await categoriesApi.remove(category.id)

    const parentNode = getParentNodeForNode(nodes.value, categoryNode)
    if (parentNode) {
      parentNode.children = parentNode.children?.filter((i) => i !== categoryNode)
    } else {
      nodes.value = nodes.value.filter((i) => i != categoryNode)
    }
  } catch (err) {
    console.error('Cannot remove category cause of', err)
  }
}

async function loadCategoryNodeContent(categoryNode: TreeNode) {
  if (categoryNode.type != 'category') return
  const category = categoryNode.data as Category

  categoryNode.children = []

  try {
    const categories = await categoriesApi.getUserCategories(category.id)
    for (const categoryDto of categories) {
      categoryNode.children?.push(categoryDtoToTreeNode(categoryDto))
    }

    const items = await itemsApi.getUserItemsFromCategory(category.id)
    for (const itemDto of items) {
      categoryNode.children?.push(itemDtoToTreeNode(itemDto))
    }

    categoryNode.children.sort(customSortingForTreeNodes)
  } catch (err) {
    console.error('Cannot get node content cause of', err)
  }
}

async function updateItemCount(itemNode: TreeNode) {
  if (itemNode.type != 'item') return
  const item = itemNode.data as Item

  try {
    await itemsApi.update(item.id, {
      name: item.name,
      count: item.count,
      categoryId: item.categoryId,
    })
  } catch (err) {
    console.error('Cannot update item count cause of', err)
  }
}

async function unfoldCategoryClickHandler(categoryNode: TreeNode) {
  if (categoryNode.type != 'category') return

  // if the node is expanded, we collapse it; otherwise, the node content will be loaded
  if (expandedKeys.value[categoryNode.key]) {
    expandedKeys.value[categoryNode.key] = false
  } else {
    try {
      categoryNode.loading = true
      await loadCategoryNodeContent(categoryNode)
    } catch (err) {
      console.error('Cannot load category node content cause of', err)
    } finally {
      categoryNode.loading = false
    }

    expandedKeys.value[categoryNode.key] = true
  }
}

async function removeNode(node: TreeNode) {
  switch (node.type) {
    case 'category': {
      await removeCategoryButtonHandler(node)
      break
    }
    case 'item': {
      await removeItemButtonHandler(node)
      break
    }
  }
}

function openChildNodeCreationPopover(event: Event, node: TreeNode) {
  popover.value?.toggle(event)
  categoryForChildCreation.value = node
}

onMounted(() => {
  loadRootCategories()
})
</script>

<template>
  <div v-if="isLoading" class="loader-container">
    <ProgressSpinner />
  </div>

  <template v-else>
    <div class="top-buttons-block">
      <Button icon="pi pi-plus" variant="text" @click="addCategoryDialogVisible = true" />
    </div>

    <Tree
      v-if="nodes.length > 0"
      :value="nodes"
      selectionMode="single"
      loadingMode="icon"
      v-model:expandedKeys="expandedKeys"
      @node-select="unfoldCategoryClickHandler"
      :pt="{
        nodeLabel: {
          style: 'width: 100%',
        },
      }"
    >
      <template #default="{ node }">
        <div class="tree-node">
          <p>{{ node.label }}</p>
          <div class="tree-node-actions">
            <InputNumber
              v-if="node.type == 'item'"
              v-model="node.data.count"
              showButtons
              buttonLayout="horizontal"
              :step="1"
              :min="0"
              :max="9999"
              :inputStyle="{ width: '4rem' }"
              @update:modelValue="updateItemCount(node)"
            />
            <Button
              v-if="node.type == 'category'"
              icon="pi pi-plus"
              variant="text"
              @click.stop="openChildNodeCreationPopover($event, node)"
            />
            <Button
              icon="pi pi-trash"
              variant="text"
              severity="danger"
              @click.stop="removeNode(node)"
            />
          </div>
        </div>
      </template>
    </Tree>

    <div v-else class="empty-list-block">
      <h1>There are no items yet. Create one!</h1>
    </div>
  </template>

  <Popover class="create-child-node-buttons-popover" ref="popover">
    <Button icon="pi pi-folder" variant="text" @click.stop="addCategoryDialogVisible = true" />
    <Button icon="pi pi-file" variant="text" @click.stop="addItemDialogVisible = true" />
  </Popover>

  <AddCategoryDialog v-model:visible="addCategoryDialogVisible" @save="addCategoryButtonHandler" />
  <AddItemDialog v-model:visible="addItemDialogVisible" @save="addItemButtonHandler" />
</template>

<style scoped>
.tree-node {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
}

.tree-node-actions {
  display: flex;
  gap: 8px;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
}

.p-tree {
  border-radius: var(--p-content-border-radius);
  padding: 0px;
}

.empty-list-block {
  display: flex;
  justify-content: center;
}

.top-buttons-block {
  display: flex;
  justify-content: end;
}

.create-child-node-buttons-popover {
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>
