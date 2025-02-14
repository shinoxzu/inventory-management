import { API_URL } from './config'
import { useTokenStore } from './stores/token'
import type {
  AuthorizedUser,
  Category,
  CreateCategoryDto,
  CreateItemDto,
  GetCategoriesResponse,
  GetItemsResponse,
  Item,
} from './types/api'

export const categoriesApi = {
  async update(categoryId: string, data: CreateCategoryDto) {
    const tokenStore = useTokenStore()

    const response = await fetch(`${API_URL}/categories/${categoryId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${tokenStore.token}`,
      },
      body: JSON.stringify(data),
    })

    ensureStatusIs(response, 200)
  },

  async remove(categoryId: string) {
    const tokenStore = useTokenStore()

    const response = await fetch(`${API_URL}/categories/${categoryId}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${tokenStore.token}`,
      },
    })

    ensureStatusIs(response, 200)
  },

  async create(createCategoryDto: CreateCategoryDto): Promise<Category> {
    const tokenStore = useTokenStore()

    const response = await fetch(`${API_URL}/categories`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${tokenStore.token}`,
      },
      body: JSON.stringify(createCategoryDto),
    })

    ensureStatusIs(response, 201)

    return await response.json()
  },

  async getUserCategories(parentCategoryId: string): Promise<Category[]> {
    const tokenStore = useTokenStore()

    const response = await fetch(`${API_URL}/categories?parentId=${parentCategoryId}`, {
      headers: {
        Authorization: `Bearer ${tokenStore.token}`,
      },
    })

    ensureStatusIs(response, 200)

    const categoriesData: GetCategoriesResponse = await response.json()
    return categoriesData.categories
  },

  async getUserRootCategories(): Promise<Category[]> {
    const tokenStore = useTokenStore()

    const response = await fetch(`${API_URL}/categories`, {
      headers: {
        Authorization: `Bearer ${tokenStore.token}`,
      },
    })

    ensureStatusIs(response, 200)

    const categoriesData: GetCategoriesResponse = await response.json()
    return categoriesData.categories
  },
}

export const itemsApi = {
  async update(itemId: string, data: CreateItemDto) {
    const tokenStore = useTokenStore()

    const response = await fetch(`${API_URL}/items/${itemId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${tokenStore.token}`,
      },
      body: JSON.stringify(data),
    })

    ensureStatusIs(response, 200)
  },

  async remove(itemId: string) {
    const tokenStore = useTokenStore()

    const response = await fetch(`${API_URL}/items/${itemId}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${tokenStore.token}`,
      },
    })

    ensureStatusIs(response, 200)
  },

  async create(createItemDto: CreateItemDto): Promise<Item> {
    const tokenStore = useTokenStore()

    const response = await fetch(`${API_URL}/items`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${tokenStore.token}`,
      },
      body: JSON.stringify(createItemDto),
    })

    ensureStatusIs(response, 201)

    return await response.json()
  },

  async getUserItemsFromCategory(categoryId: string): Promise<Item[]> {
    const tokenStore = useTokenStore()

    const response = await fetch(`${API_URL}/items?categoryId=${categoryId}`, {
      headers: {
        Authorization: `Bearer ${tokenStore.token}`,
      },
    })

    ensureStatusIs(response, 200)

    const itemsData: GetItemsResponse = await response.json()
    return itemsData.items
  },
}

export const authApi = {
  async authorizeWithGitHub(code: string): Promise<AuthorizedUser> {
    const response = await fetch(
      `${API_URL}/auth/github?` +
        new URLSearchParams({
          code: code,
        }).toString(),
    )

    ensureStatusIs(response, 200)

    return await response.json()
  },
}

function ensureStatusIs(response: Response, status: number) {
  if (response.status != status) {
    throw new Error(`cannot update category; status is ${response.status}`)
  }
}
