import { Navigate, useLocation } from 'react-router-dom'
import type { ReactElement } from 'react'
import { useAuthToken } from '../hooks/useAuthToken'
import { getRoleFromJwt } from '../utils/jwt'

const AdminProtectedRoute = ({ children }: { children: ReactElement }) => {
  const token = useAuthToken()
  const location = useLocation()
  const role = getRoleFromJwt(token)

  if (role !== 'Admin') {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  return children
}

export default AdminProtectedRoute
