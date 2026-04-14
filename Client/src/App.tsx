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
import UserListPage from './pages/UserListPage.tsx'
import UserDetailPage from './pages/UserDetailPage.tsx'
import { useTheme } from './hooks/useTheme.ts'

function App() {
  const { theme, toggleTheme } = useTheme()

  return (
    <div className="app-root egg-shell min-h-screen py-8">
      <div className="app-inner w-full max-w-6xl mx-auto px-4">
        <div className="theme-toggle-row">
          <button
            type="button"
            className={`theme-switch ${theme === 'alternate' ? 'is-alternate' : ''}`}
            onClick={toggleTheme}
            role="switch"
            aria-checked={theme === 'alternate'}
            aria-label={theme === 'default' ? 'Switch to alternate theme' : 'Switch to default theme'}
          >
            <span className="theme-switch__track" aria-hidden="true">
              <span className="theme-switch__icon theme-switch__icon--moon">
                <svg viewBox="0 0 24 24" focusable="false" aria-hidden="true">
                  <path d="M20 15.31A8.5 8.5 0 1 1 8.69 4a7 7 0 1 0 11.31 11.31Z" />
                </svg>
              </span>
              <span className="theme-switch__icon theme-switch__icon--sun">
                <svg viewBox="0 0 24 24" focusable="false" aria-hidden="true">
                  <circle cx="12" cy="12" r="4" />
                  <path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M4.93 19.07l1.41-1.41M17.66 6.34l1.41-1.41" />
                </svg>
              </span>
              <span className="theme-switch__thumb" />
            </span>
          </button>
        </div>
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
              <ProtectedRoute>
                <LeaderboardsPage />
              </ProtectedRoute>
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
