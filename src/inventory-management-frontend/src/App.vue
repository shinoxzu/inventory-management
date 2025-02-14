<script setup lang="ts">
import { ref, watchEffect, type Ref } from 'vue'
import { useRouter, RouterView } from 'vue-router'
import { Menubar } from 'primevue'
import type { MenuItem } from 'primevue/menuitem'
import { useAuthStore } from './stores/auth'

const authStore = useAuthStore()
const router = useRouter()

const items: Ref<MenuItem[]> = ref([])

function updateMenuItems() {
  const menuItems = []

  if (!authStore.isAuthorized) {
    menuItems.push({
      label: 'Login',
      icon: 'pi pi-user',
      command: () => {
        router.push('/login')
      },
    })
  } else {
    menuItems.push({
      label: 'Logout',
      icon: 'pi pi-sign-out',
      command: () => {
        authStore.deauthorize()
        router.push('/login')
      },
    })
  }

  items.value = menuItems
}

watchEffect(() => {
  updateMenuItems()
})
</script>

<template>
  <header>
    <nav>
      <Menubar :model="items">
        <template #start>
          <RouterLink to="/" style="text-decoration: none; color: var(--p-text-color)">
            <h3>InventoryManagement</h3>
          </RouterLink>
        </template>
      </Menubar>
    </nav>
  </header>

  <main>
    <RouterView />
  </main>
</template>

<style scoped>
main {
  margin-top: 25px;
}
@media (min-width: 768px) {
  main {
    margin-left: 10%;
    margin-right: 10%;
  }
}
</style>
