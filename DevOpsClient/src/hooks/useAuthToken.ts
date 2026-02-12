const TOKEN_KEY = 'accessToken'

export const useAuthToken = () => {
  return localStorage.getItem(TOKEN_KEY)
}
