import type { TreeNode } from 'primevue/treenode'
import type { Category, Item } from '@/types/api'

export function getParentNodeForNode(nodes: TreeNode[], target: TreeNode): TreeNode | null {
  function findParent(root: TreeNode): TreeNode | null {
    if (root === target) return null

    for (const child of root.children || []) {
      if (child === target) return root
      const result = findParent(child)
      if (result) return result
    }

    return null
  }

  for (const node of nodes) {
    const result = findParent(node)
    if (result) return result
  }

  return null
}

export function itemDtoToTreeNode(item: Item): TreeNode {
  return {
    key: `item_${item.id}`,
    label: item.name,
    icon: 'pi pi-file',
    type: 'item',
    children: [],
    data: item,
  }
}

export function categoryDtoToTreeNode(category: Category): TreeNode {
  return {
    key: `category_${category.id}`,
    label: category.name,
    icon: 'pi pi-folder',
    type: 'category',
    leaf: false,
    children: [],
    data: category,
  }
}

export function customSortingForTreeNodes(node1: TreeNode, node2: TreeNode): number {
  if (node1.type == 'category' && node2.type == 'item') {
    return -1
  } else if (node1.type == 'item' && node2.type == 'category') {
    return 1
  } else {
    if (!node1.label || !node2.label) {
      return 0
    }

    return node1.label < node2.label ? -1 : 1
  }
}
