import { Route, Routes } from 'react-router-dom'
import './App.css'
import ProtectedRoute from './components/ProtectedRoute.tsx'
import AccountSettingsPage from './pages/AccountSettingsPage.tsx'
import LeaderboardsPage from './pages/LeaderboardsPage.tsx'
import LoginPage from './pages/LoginPage.tsx'
import MainPage from './pages/MainPage.tsx'
import RegisterPage from './pages/RegisterPage.tsx'

function App() {
  return (
    <div className="app-root egg-shell min-h-screen py-8">
      <div className="app-inner w-full max-w-6xl mx-auto px-4">
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
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
        </Routes>
      </div>
    </div>
  )
}

export default App
