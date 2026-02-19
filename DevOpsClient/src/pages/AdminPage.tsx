import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { userApi, type UserProfile } from '../api/userApi'
import { useAuthToken } from '../hooks/useAuthToken'
import { getRoleFromJwt } from '../utils/jwt'

const AdminPage = () => {
  const token = useAuthToken()
  const role = getRoleFromJwt(token)
  const [profile, setProfile] = useState<UserProfile | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadProfile = async () => {
      if (!token) {
        setError('Access token is missing in localStorage (accessToken).')
        return
      }

      try {
        const me = await userApi.getCurrentUser(token)
        setProfile(me)
        setError(null)
      } catch (err) {
        const message = err instanceof Error ? err.message : 'Failed to load admin data.'
        setError(message)
      }
    }

    void loadProfile()
  }, [token])

  const adminSummary = useMemo(
    () => [
      { label: 'Signed in as', value: profile?.username ?? 'Loading...' },
      { label: 'Email', value: profile?.email ?? 'Loading...' },
      { label: 'Role', value: role ?? 'Unknown' }
    ],
    [profile?.email, profile?.username, role]
  )

  const isAdmin = (role ?? '').toLowerCase().includes('admin')

  return (
    <div className="leaderboard-stage">
      <header className="shell-panel rise-in leaderboard-header">
        <div>
          <p className="eyebrow">Admin Center</p>
          <h1 className="hero-title">Control Room</h1>
          <p className="hero-sub">Overview your profile, privileges, and jump into user management.</p>
          <div className="leaderboard-summary mt-4">
            {adminSummary.map((item) => (
              <div key={item.label} className="leaderboard-summary__card">
                <p className="stat-label">{item.label}</p>
                <p className="stat-value">{item.value}</p>
              </div>
            ))}
          </div>
        </div>
        <div className="leaderboard-header__actions">
          <Link to="/" className="btn btn-outline">
            Back to dashboard
          </Link>
          <Link to="/admin/users" className="btn btn-primary">
            Open UserList
          </Link>
          {!isAdmin ? <span className="leaderboard-meta__text">Admin role required for management tools.</span> : null}
        </div>
      </header>

      <section className="leaderboard-grid">
        <div className="shell-panel leaderboard-card rise-in">
          <div className="leaderboard-card__header">
            <div>
              <p className="eyebrow">User operations</p>
              <h3 className="panel-title">Manage users</h3>
            </div>
            <Link to="/admin/users" className="btn btn-sm btn-primary">
              Go to UserList
            </Link>
          </div>
          <p className="leaderboard-card__meta">
            Review registrations, inspect roles, and take actions on any account from the UserList.
          </p>
        </div>

        <div className="shell-panel leaderboard-card rise-in">
          <div className="leaderboard-card__header">
            <div>
              <p className="eyebrow">Shortcuts</p>
              <h3 className="panel-title">Other areas</h3>
            </div>
            <div className="shell-actions">
              <Link to="/leaderboards" className="btn btn-sm btn-outline">
                Leaderboards
              </Link>
              <Link to="/settings" className="btn btn-sm btn-outline">
                Account settings
              </Link>
            </div>
          </div>
          <p className="leaderboard-card__meta">
            Hop to other dashboards without leaving the admin shell.
          </p>
        </div>
      </section>

      {error ? <p className="text-error text-sm">{error}</p> : null}
      {!token ? <p className="text-error text-sm">Sign in again to access admin tools.</p> : null}
    </div>
  )
}

export default AdminPage
