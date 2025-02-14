<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { Button, ProgressSpinner } from 'primevue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const route = useRoute()
const router = useRouter()

const isLoading = ref<boolean>(false)
const githubLoginUrl = computed<string>(() => authStore.generateAuthUrl())

async function tryFetchGitHubCodeAndAuthorize() {
  const maybeCode = route.query.code
  const maybeState = route.query.state

  if (maybeCode && maybeState) {
    try {
      isLoading.value = true
      await authStore.authorizeWithGitHubAuthCode(maybeCode.toString(), maybeState.toString())
      router.push('/')
    } catch (err) {
      console.error('Cannot fetch token cause of', err)
    } finally {
      isLoading.value = false
    }
  }
}

onMounted(() => {
  tryFetchGitHubCodeAndAuthorize()
})
</script>

<template>
  <div v-if="isLoading" class="loader-container">
    <ProgressSpinner />
  </div>

  <div v-else class="login-buttons-container">
    <Button
      icon="pi pi-github"
      style="text-decoration: none"
      label="Login with GitHub"
      size="large"
      as="a"
      :href="githubLoginUrl"
    />
  </div>
</template>

<style scoped>
.login-buttons-container {
  display: flex;
  align-content: center;
  flex-direction: column;
  gap: 15px;
}
</style>
