import { Navigate } from 'react-router-dom'
import type { ReactElement } from 'react'
import { featureFlags } from '../utils/featureFlags'
import ProtectedRoute from './ProtectedRoute'

const LeaderboardsProtectedRoute = ({ children }: { children: ReactElement }) => {
  if (!featureFlags.leaderboards) {
    return <Navigate to="/" replace />
  }

  return <ProtectedRoute>{children}</ProtectedRoute>
}

export default LeaderboardsProtectedRoute
