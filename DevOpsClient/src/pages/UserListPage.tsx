import { Link } from 'react-router-dom'
import { useMemo } from 'react'
import { useAdminUsers, type SortField } from '../hooks/useAdminUsers'

const UserListPage = () => {
  const {
    users,
    total,
    page,
    totalPages,
    visibleStart,
    visibleEnd,
    loading,
    hasLoadedOnce,
    error,
    searchTerm,
    roleFilter,
    sortField,
    sortDirection,
    setSearchTerm,
    setRoleFilter,
    toggleSort,
    handlePageChange,
    resetFilters,
    refresh
  } = useAdminUsers()

  const sortIndicator = useMemo(
    () =>
      (field: SortField) => {
        if (sortField !== field) return '↕'
        return sortDirection === 'asc' ? '↑' : '↓'
      },
    [sortDirection, sortField]
  )

  return (
    <div className="leaderboard-stage">
      <header className="shell-panel rise-in leaderboard-header">
        <div>
          <p className="eyebrow">Admin Center</p>
          <h1 className="hero-title">User Directory</h1>
          <p className="hero-sub">Search, sort, and page through all registered users.</p>
          <div className="leaderboard-summary mt-4">
            <div className="leaderboard-summary__card">
              <p className="stat-label">Total users</p>
              <p className="stat-value">{total}</p>
            </div>
            <div className="leaderboard-summary__card">
              <p className="stat-label">Showing</p>
              <p className="stat-value">
                {total === 0 ? '0' : `${visibleStart}-${visibleEnd}`} of {total}
              </p>
            </div>
            <div className="leaderboard-summary__card">
              <p className="stat-label">Page</p>
              <p className="stat-value">
                {page}/{totalPages}
              </p>
            </div>
          </div>
        </div>
        <div className="leaderboard-header__actions">
          <Link to="/AdminPage" className="btn btn-outline">
            Back to Admin
          </Link>
          <button className="btn btn-primary" onClick={() => void refresh()} disabled={loading}>
            {loading ? 'Refreshing…' : 'Refresh list'}
          </button>
        </div>
      </header>

      <section className="shell-panel leaderboard-card rise-in">
        <div className="leaderboard-card__header">
          <div>
            <p className="eyebrow">Directory</p>
            <h3 className="panel-title">All users</h3>
            <p className="leaderboard-card__meta">Use the search, role filter, and sortable columns to inspect users.</p>
          </div>
          <div className="shell-actions">
            <input
              className="input input-bordered w-48"
              placeholder="Search username or email"
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
            />
            <select
              className="select select-bordered w-32"
              value={roleFilter}
              onChange={(event) => setRoleFilter(event.target.value as Parameters<typeof setRoleFilter>[0])}
            >
              <option value="All">All roles</option>
              <option value="Admin">Admin</option>
              <option value="User">User</option>
            </select>
            <button className="btn btn-outline btn-sm" onClick={resetFilters} disabled={loading && !hasLoadedOnce}>
              Reset filters
            </button>
          </div>
        </div>

        {error ? <p className="text-error text-sm">{error}</p> : null}
        {loading && !hasLoadedOnce ? <p className="text-sm opacity-70">Loading users…</p> : null}

        {!loading && hasLoadedOnce && users.length === 0 ? (
          <p className="text-sm opacity-70">No users match the current filters.</p>
        ) : null}

        {users.length > 0 ? (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead>
                <tr className="border-b border-gray-700/50">
                  <th className="py-2 pr-4">
                    <button className="btn btn-xs btn-outline" onClick={() => toggleSort('username')}>
                      Username {sortIndicator('username')}
                    </button>
                  </th>
                  <th className="py-2 pr-4">
                    <button className="btn btn-xs btn-outline" onClick={() => toggleSort('email')}>
                      Email {sortIndicator('email')}
                    </button>
                  </th>
                  <th className="py-2 pr-4">Role</th>
                  <th className="py-2 pr-4">ID</th>
                </tr>
              </thead>
              <tbody>
                {users.map((user, index) => (
                  <tr
                    key={user.userId ?? `${user.username ?? user.email ?? 'user'}-${index}`}
                    className="border-b border-gray-700/30"
                  >
                    <td className="py-2 pr-4 font-semibold">{user.username ?? 'Unknown'}</td>
                    <td className="py-2 pr-4">{user.email ?? '—'}</td>
                    <td className="py-2 pr-4">{user.role ?? '—'}</td>
                    <td className="py-2 pr-4 text-xs text-gray-300/80">{user.userId ?? '—'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : null}

        <div className="shell-actions mt-4">
          <div className="text-sm opacity-80">
            {total === 0 ? 'No users to display' : `Showing ${visibleStart}-${visibleEnd} of ${total}`}
          </div>
          <div className="shell-actions">
            <button className="btn btn-outline btn-sm" onClick={() => handlePageChange('prev')} disabled={loading || page <= 1}>
              Previous
            </button>
            <span className="text-sm opacity-80">Page {page} of {totalPages}</span>
            <button
              className="btn btn-outline btn-sm"
              onClick={() => handlePageChange('next')}
              disabled={loading || page >= totalPages}
            >
              Next
            </button>
          </div>
        </div>
      </section>
    </div>
  )
}

export default UserListPage
