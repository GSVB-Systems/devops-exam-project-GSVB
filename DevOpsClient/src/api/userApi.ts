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

const login = async (request: LoginRequest): Promise<AuthResponse> => {
  const response = await fetch('/api/User/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(request)
  })

  if (!response.ok) {
    const message = await response.text()
    const error: ApiError = {
      message: message || `Request failed with status ${response.status}`,
      status: response.status
    }
    throw error
  }

  return (await response.json()) as AuthResponse
}

export const userApi = {
  login
}

