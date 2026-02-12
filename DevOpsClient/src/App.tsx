import { Route, Routes } from 'react-router-dom'
import './App.css'
import ProtectedRoute from './components/ProtectedRoute'
import AccountSettingsPage from './pages/AccountSettingsPage.tsx'
import LoginPage from './pages/LoginPage.tsx'
import MainPage from './pages/MainPage.tsx'

function App() {
  return (
    <div className="app-root min-h-screen bg-base-200 py-10 flex items-center justify-center">
      <div className="app-inner w-full px-4 space-y-6">
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
          <Route path="/login" element={<LoginPage />} />
        </Routes>
      </div>
    </div>
  )
}

export default App
