import { computed } from 'vue'
import { defineStore } from 'pinia'
import { useSessionStorage } from '@vueuse/core'
import type { User } from '@/types/api'
import { authApi } from '@/api'
import { generateState } from '@/utils/crypto'
import {
  GITHUB_AUTH_BASE_URL,
  GITHUB_AUTH_CLIENT_ID,
  GITHUB_AUTH_REDIRECT_URI,
  GITHUB_AUTH_SCOPE,
} from '@/config'
import { useTokenStore } from './token'

export const useAuthStore = defineStore('auth', () => {
  const tokenStore = useTokenStore()
  const oauthState = useSessionStorage<string | null>('oauthState', null)
  const isAuthorized = computed<boolean>(() => tokenStore.token !== null)

  function deauthorize() {
    oauthState.value = null
    tokenStore.token = null
  }

  async function authorizeWithGitHubAuthCode(code: string, state: string): Promise<User> {
    if (oauthState.value == null || state !== oauthState.value) {
      throw new Error(`Invalid oauth state (passed: ${state}, expected: ${oauthState.value})`)
    }

    const authorizedUser = await authApi.authorizeWithGitHub(code)

    oauthState.value = null
    tokenStore.token = authorizedUser.token

    return authorizedUser.user
  }

  function generateAuthUrl(): string {
    if (oauthState.value == null) {
      oauthState.value = generateState()
    }

    const params = new URLSearchParams({
      client_id: GITHUB_AUTH_CLIENT_ID,
      scope: GITHUB_AUTH_SCOPE,
      redirect_uri: GITHUB_AUTH_REDIRECT_URI,
      state: oauthState.value,
    })

    return `${GITHUB_AUTH_BASE_URL}?${params}`
  }

  return {
    isAuthorized,
    deauthorize,
    authorizeWithGitHubAuthCode,
    generateAuthUrl,
  }
})
