import {Link, useNavigate, useParams} from 'react-router-dom'
import {useAdminUserDetails} from '../hooks/useAdminUserDetails'
import type {FormEvent} from 'react'
import {useState} from 'react'

const UserDetailPage = () => {
  const { userId } = useParams<{ userId: string }>()
  const navigate = useNavigate()

  const [passwordConfirm, setPasswordConfirm] = useState('')
  const [validationError, setValidationError] = useState<string | null>(null)

  const { user, form, loading, saving, deleting, hasLoadedOnce, error, setField, refresh, updateUser, deleteUser } = useAdminUserDetails(userId)

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()
    if (form.password.trim() && form.password !== passwordConfirm) {
      setValidationError('Passwords do not match')
      return
    }
    setValidationError(null)
    await updateUser()
    setPasswordConfirm('')
  }

  const handleDelete = async () => {
    if (!userId) return
    const confirmed = window.confirm('Are you sure you want to delete this user? This cannot be undone.')
    if (!confirmed) return
    await deleteUser()
    navigate('/admin/users')
  }

  if (!userId) {
    return (
      <div className="leaderboard-stage">
        <p className="text-error">No user id provided.</p>
        <Link to="/admin/users" className="btn btn-outline mt-2">
          Back to user list
        </Link>
      </div>
    )
  }

  return (
    <div className="leaderboard-stage">
      <header className="shell-panel rise-in leaderboard-header">
        <div>
          <p className="eyebrow">Admin Center</p>
          <h1 className="hero-title">User details</h1>
          <p className="hero-sub">Inspect and manage a single account.</p>
          <div className="leaderboard-summary mt-4">
            <div className="leaderboard-summary__card">
              <p className="stat-label">Username</p>
              <p className="stat-value">{user?.username ?? '—'}</p>
            </div>
            <div className="leaderboard-summary__card">
              <p className="stat-label">Role</p>
              <p className="stat-value">{user?.role ?? '—'}</p>
            </div>
            <div className="leaderboard-summary__card">
              <p className="stat-label">User ID</p>
              <p className="stat-value text-xs">{user?.userId ?? userId}</p>
            </div>
          </div>
        </div>
        <div className="leaderboard-header__actions">
          <Link to="/admin/users" className="btn btn-outline">
            Back to list
          </Link>
          <button className="btn btn-primary" onClick={() => void refresh()} disabled={loading || deleting || saving}>
            {loading ? 'Refreshing…' : 'Refresh'}
          </button>
        </div>
      </header>

      <section className="shell-panel leaderboard-card rise-in">
        <div className="leaderboard-card__header">
          <div>
            <p className="eyebrow">Account</p>
            <h3 className="panel-title">Profile & access</h3>
            <p className="leaderboard-card__meta">Edit details or remove the account.</p>
          </div>
        </div>

        {error ? <p className="text-error text-sm mb-3">{error}</p> : null}
        {validationError ? <p className="text-error text-sm mb-3">{validationError}</p> : null}
        {loading && !hasLoadedOnce ? <p className="text-sm opacity-70">Loading user…</p> : null}

        <form className="space-y-4" onSubmit={handleSubmit}>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <label className="form-control w-full">
              <span className="label-text">Username</span>
              <input
                className="input input-bordered"
                value={form.username}
                onChange={(event) => setField('username', event.target.value)}
                disabled={saving || deleting || loading}
              />
            </label>

            <label className="form-control w-full">
              <span className="label-text">Email</span>
              <input
                type="email"
                className="input input-bordered"
                value={form.email}
                onChange={(event) => setField('email', event.target.value)}
                disabled={saving || deleting || loading}
              />
            </label>

            <label className="form-control w-full">
              <span className="label-text">Discord username</span>
              <input
                className="input input-bordered"
                value={form.discordUsername}
                onChange={(event) => setField('discordUsername', event.target.value)}
                disabled={saving || deleting || loading}
              />
            </label>

            <label className="form-control w-full">
              <span className="label-text">Role</span>
              <select
                className="select select-bordered"
                value={form.role}
                onChange={(event) => setField('role', event.target.value)}
                disabled={saving || deleting || loading}
              >
                <option value="User">User</option>
                <option value="Admin">Admin</option>
              </select>
            </label>

            <label className="form-control w-full md:col-span-2">
              <span className="label-text">Password (leave blank to keep current)</span>
              <input
                type="password"
                className="input input-bordered"
                value={form.password}
                onChange={(event) => setField('password', event.target.value)}
                disabled={saving || deleting || loading}
              />
            </label>

            <label className="form-control w-full md:col-span-2">
              <span className="label-text">Confirm password</span>
              <input
                type="password"
                className="input input-bordered"
                value={passwordConfirm}
                onChange={(event) => setPasswordConfirm(event.target.value)}
                disabled={saving || deleting || loading}
              />
            </label>
          </div>

          <div className="shell-actions">
            <button type="submit" className="btn btn-primary" disabled={saving || deleting || loading}>
              {saving ? 'Saving…' : 'Save changes'}
            </button>
            <button type="button" className="btn btn-error btn-outline" onClick={() => void handleDelete()} disabled={deleting || saving}>
              {deleting ? 'Deleting…' : 'Delete user'}
            </button>
          </div>
        </form>
      </section>
    </div>
  )
}

export default UserDetailPage
