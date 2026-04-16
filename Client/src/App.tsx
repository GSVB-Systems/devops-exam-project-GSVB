import { Route, Routes } from 'react-router-dom'
import './App.css'
import ProtectedRoute from './components/ProtectedRoute.tsx'
import AccountSettingsPage from './pages/AccountSettingsPage.tsx'
import LeaderboardsPage from './pages/LeaderboardsPage.tsx'
import LoginPage from './pages/LoginPage.tsx'
import MainPage from './pages/MainPage.tsx'
import RegisterPage from './pages/RegisterPage.tsx'
import AdminDashboard from './pages/Dashboard.tsx'
import AdminPage from './pages/AdminPage.tsx'
import AdminProtectedRoute from './components/AdminProtectedRoute.tsx'
import LeaderboardsProtectedRoute from './components/LeaderboardsProtectedRoute.tsx'
import UserListPage from './pages/UserListPage.tsx'
import UserDetailPage from './pages/UserDetailPage.tsx'
import ThemeToggle from './components/ThemeToggle.tsx'

function App() {
  return (
    <div className="app-root egg-shell min-h-screen py-8">
      <div className="app-inner w-full max-w-6xl mx-auto px-4">
        <ThemeToggle />
        <Routes>
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <MainPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/settings"
            element={
              <ProtectedRoute>
                <AccountSettingsPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/leaderboards"
            element={
              <LeaderboardsProtectedRoute>
                <LeaderboardsPage />
              </LeaderboardsProtectedRoute>
            }
          />
          <Route
                path="/dashboard"
                element={
                    <ProtectedRoute>
                        <AdminDashboard />
                    </ProtectedRoute>
                }
          />
          <Route
            path="/AdminPage"
            element={
              <AdminProtectedRoute>
                <AdminPage />
              </AdminProtectedRoute>
            }
          />
          <Route
            path="/admin/users"
            element={
              <AdminProtectedRoute>
                <UserListPage />
              </AdminProtectedRoute>
            }
          />
          <Route
            path="/admin/users/:userId"
            element={
              <AdminProtectedRoute>
                <UserDetailPage />
              </AdminProtectedRoute>
            }
          />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
        </Routes>
      </div>
    </div>
  )
}

export default App
