import { Navigate, useLocation } from 'react-router-dom'
import type { ReactElement } from 'react'
import { useAuthToken } from '../hooks/useAuthToken'

const ProtectedRoute = ({ children }: { children: ReactElement }) => {
  const token = useAuthToken()
  const location = useLocation()

  if (!token) {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  return children
}

export default ProtectedRoute
