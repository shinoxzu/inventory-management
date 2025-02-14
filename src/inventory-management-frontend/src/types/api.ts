export interface Item {
  id: string
  name: string
  count: number
  authorId: string
  categoryId: string
}

export interface CreateItemDto {
  name: string
  count: number
  categoryId: string
}

export interface Category {
  id: string
  name: string
  parentId: string | null
}

export interface CreateCategoryDto {
  name: string
  parentId: string | null
}

export interface GetItemsResponse {
  totalCount: number
  items: Item[]
}

export interface GetCategoriesResponse {
  totalCount: number
  categories: Category[]
}

export interface User {
  id: string
  name: string
}

export interface AuthorizedUser {
  token: string
  user: User
}
