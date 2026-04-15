import { useCallback, useState } from 'react'
import { userApi, type ApiError, type AuthResponse, type LoginRequest } from '../api/userApi'

type UseLoginResult = {
  login: (request: LoginRequest) => Promise<AuthResponse | null>
  isLoading: boolean
  error: string | null
  token: string | null
  expiresInSeconds: number | null
}

export const useLogin = (): UseLoginResult => {
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [token, setToken] = useState<string | null>(null)
  const [expiresInSeconds, setExpiresInSeconds] = useState<number | null>(null)

  const login = useCallback(async (request: LoginRequest) => {
    setIsLoading(true)
    setError(null)

    try {
      const response = await userApi.login(request)
      setToken(response.accessToken)
      setExpiresInSeconds(response.expiresInSeconds)
      localStorage.setItem('accessToken', response.accessToken)
      return response
    } catch (err) {
      const message = (err as ApiError).message ?? 'Login failed.'
      setError(message)
      return null
    } finally {
      setIsLoading(false)
    }
  }, [])

  return {
    login,
    isLoading,
    error,
    token,
    expiresInSeconds
  }
}
