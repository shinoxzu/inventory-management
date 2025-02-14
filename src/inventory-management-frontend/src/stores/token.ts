import { defineStore } from 'pinia'
import { useLocalStorage } from '@vueuse/core'

export const useTokenStore = defineStore('token', () => {
  const token = useLocalStorage<string | null>('token', null)
  return { token }
})
