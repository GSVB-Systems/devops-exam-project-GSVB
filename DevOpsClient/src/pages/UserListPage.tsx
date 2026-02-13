import { Link } from 'react-router-dom'

const UserListPage = () => {
  return (
    <div className="leaderboard-stage">
      <header className="shell-panel rise-in leaderboard-header">
        <div>
          <p className="eyebrow">Admin Center</p>
          <h1 className="hero-title">UserList</h1>
          <p className="hero-sub">Manage all registered users from one place.</p>
        </div>
        <div className="leaderboard-header__actions">
          <Link to="/AdminPage" className="btn btn-outline">
            Back to Admin
          </Link>
        </div>
      </header>

      <section className="shell-panel leaderboard-card rise-in">
        <div className="leaderboard-card__header">
          <div>
            <p className="eyebrow">Coming soon</p>
            <h3 className="panel-title">User management tools</h3>
          </div>
        </div>
        <p className="leaderboard-card__meta">
          This is a placeholder for the full user management experience. Add the list, filters, and actions once the API is ready.
        </p>
      </section>
    </div>
  )
}

export default UserListPage
