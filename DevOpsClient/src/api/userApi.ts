export type LoginRequest = {
  email: string
  password: string
}

export type AuthResponse = {
  accessToken: string
  tokenType: string
  expiresInSeconds: number
}

export type ApiError = {
  message: string
  status?: number
}

export type UserProfile = {
  userId: string
  username: string
  discordUsername?: string | null
  email: string
}

export type UpdateUserRequest = {
  username?: string
  discordUsername?: string | null
  email?: string
  password?: string
}

export type EggAccount = {
  id: string
  eiUserId: string
  status: 'Main' | 'Alt'
  userName?: string | null
  lastFetchedUtc?: string | null
}

export type CreateEggAccountRequest = {
  eiUserId: string
  status?: 'Main' | 'Alt'
}

export type UpdateEggAccountRequest = {
  status?: 'Main' | 'Alt'
}

const ensureOk = async (response: Response) => {
  if (!response.ok) {
    const message = await response.text()
    const error: ApiError = {
      message: message || `Request failed with status ${response.status}`,
      status: response.status
    }
    throw error
  }
}

const login = async (request: LoginRequest): Promise<AuthResponse> => {
  const response = await fetch('/api/User/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(request)
  })

  await ensureOk(response)

  return (await response.json()) as AuthResponse
}

const getCurrentUser = async (token: string): Promise<UserProfile> => {
  const response = await fetch('/api/User/me', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  await ensureOk(response)
  return (await response.json()) as UserProfile
}

const updateCurrentUser = async (token: string, request: UpdateUserRequest): Promise<UserProfile> => {
  const response = await fetch('/api/User/me', {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(request)
  })

  await ensureOk(response)
  return (await response.json()) as UserProfile
}

const getEggAccounts = async (token: string): Promise<EggAccount[]> => {
  const response = await fetch('/api/egg-accounts', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  await ensureOk(response)
  return (await response.json()) as EggAccount[]
}

const createEggAccount = async (token: string, request: CreateEggAccountRequest): Promise<EggAccount> => {
  const response = await fetch('/api/egg-accounts', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(request)
  })

  await ensureOk(response)
  return (await response.json()) as EggAccount
}

const updateEggAccount = async (token: string, id: string, request: UpdateEggAccountRequest): Promise<EggAccount> => {
  const response = await fetch(`/api/egg-accounts/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(request)
  })

  await ensureOk(response)
  return (await response.json()) as EggAccount
}

const deleteEggAccount = async (token: string, id: string): Promise<void> => {
  const response = await fetch(`/api/egg-accounts/${encodeURIComponent(id)}`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  await ensureOk(response)
}

export const userApi = {
  login,
  getCurrentUser,
  updateCurrentUser,
  getEggAccounts,
  createEggAccount,
  updateEggAccount,
  deleteEggAccount
}

