export type JwtPayload = Record<string, unknown>

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

export const decodeJwt = (token: string): JwtPayload | null => {
  try {
    const payloadBase64 = token.split('.')[1]
    const base64 = payloadBase64.replace(/-/g, '+').replace(/_/g, '/')
    const json = decodeURIComponent(
      atob(base64)
        .split('')
        .map((char) => `%${`00${char.charCodeAt(0).toString(16)}`.slice(-2)}`)
        .join('')
    )
    return JSON.parse(json) as JwtPayload
  } catch {
    return null
  }
}

export const getRoleFromJwt = (token: string | null): string | null => {
  if (!token) {
    return null
  }

  const payload = decodeJwt(token)
  if (!payload) {
    return null
  }

  const claim = payload[ROLE_CLAIM] ?? payload.role
  return typeof claim === 'string' ? claim : null
}
