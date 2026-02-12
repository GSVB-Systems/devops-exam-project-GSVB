import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { userApi, type EggAccount, type EggAccountRefresh, type UserProfile } from '../api/userApi'

const MainPage = () => {
  const [profile, setProfile] = useState<UserProfile | null>(null)
  const [eggAccounts, setEggAccounts] = useState<EggAccount[]>([])
  const [selectedEiUserId, setSelectedEiUserId] = useState<string | null>(null)
  const [snapshot, setSnapshot] = useState<EggAccountRefresh | null>(null)
  const [isRefreshing, setIsRefreshing] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [refreshError, setRefreshError] = useState<string | null>(null)

  useEffect(() => {
    const loadAccounts = async () => {
      const token = localStorage.getItem('accessToken')
      if (!token) {
        setError('Access token is missing in localStorage (accessToken).')
        return
      }

      try {
        const [user, accounts] = await Promise.all([userApi.getCurrentUser(token), userApi.getEggAccounts(token)])
        setProfile(user)
        setEggAccounts(accounts)
        const mainAccount = accounts.find((account) => account.status === 'Main')
        const defaultEiUserId = mainAccount?.eiUserId ?? accounts[0]?.eiUserId ?? null
        setSelectedEiUserId(defaultEiUserId)
      } catch (err) {
        const message = err instanceof Error ? err.message : 'Failed to load egg accounts.'
        setError(message)
      }
    }

    void loadAccounts()
  }, [])

  useEffect(() => {
    setSnapshot(null)
    setRefreshError(null)
  }, [selectedEiUserId])

  const selectedAccount = useMemo(
    () => eggAccounts.find((account) => account.eiUserId === selectedEiUserId) ?? null,
    [eggAccounts, selectedEiUserId]
  )

  const formatNumber = (value: number | null | undefined, digits = 2) => {
    if (value === null || value === undefined) return '--'
    return value.toLocaleString(undefined, { maximumFractionDigits: digits })
  }

  const formatSoulEggs = (value: number | null | undefined) => {
    if (value === null || value === undefined) return '--'
    if (value >= 1e18) {
      return `${(value / 1e18).toFixed(2)}Q`
    }
    return value.toLocaleString()
  }

  const formatDate = (value: string | null | undefined) => {
    if (!value) return 'Not synced yet'
    const date = new Date(value)
    return Number.isNaN(date.getTime()) ? 'Not synced yet' : date.toLocaleString()
  }

  const handleRefresh = async () => {
    if (!selectedEiUserId) {
      setRefreshError('Select an egg account first.')
      return
    }

    const token = localStorage.getItem('accessToken')
    if (!token) {
      setRefreshError('Access token is missing in localStorage (accessToken).')
      return
    }

    setIsRefreshing(true)
    setRefreshError(null)

    try {
      const data = await userApi.refreshEggAccount(token, selectedEiUserId)
      setSnapshot(data)
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to refresh snapshot.'
      setRefreshError(message)
    } finally {
      setIsRefreshing(false)
    }
  }

  const displayName = selectedAccount?.userName ?? selectedAccount?.eiUserId ?? 'No account selected'
  const welcomeName = profile?.username ?? 'Farmer'

  return (
    <div className="egg-stage">
      <aside className="shell-panel shell-panel--side rise-in">
        <div className="flex items-center justify-between">
          <div>
            <p className="eyebrow">Account Switcher</p>
            <h3 className="panel-title">Egg Accounts</h3>
          </div>
          <Link to="/settings" className="btn btn-xs btn-outline">
            Settings
          </Link>
        </div>

        <div className="mt-4 lg:hidden">
          <select
            className="select select-bordered w-full"
            value={selectedEiUserId ?? ''}
            onChange={(event) => setSelectedEiUserId(event.target.value || null)}
          >
            <option value="" disabled>
              Select an egg account
            </option>
            {eggAccounts.map((account) => (
              <option key={account.id} value={account.eiUserId}>
                {account.userName ? account.userName : account.eiUserId} ({account.status})
              </option>
            ))}
          </select>
        </div>

        <div className="account-list hidden lg:flex">
          {eggAccounts.length === 0 ? (
            <p className="text-sm opacity-70">No linked accounts yet.</p>
          ) : (
            eggAccounts.map((account) => (
              <button
                key={account.id}
                type="button"
                className={`account-pill ${account.eiUserId === selectedEiUserId ? 'active' : ''}`}
                onClick={() => setSelectedEiUserId(account.eiUserId)}
              >
                <span className="account-pill__name">{account.userName ?? account.eiUserId}</span>
                <span className="account-pill__meta">{account.status}</span>
              </button>
            ))
          )}
        </div>

        {error ? <p className="text-error text-sm mt-4">{error}</p> : null}
      </aside>

      <section className="shell-panel shell-panel--main rise-in">
        <div className="stage-header">
          <div>
            <p className="eyebrow">Welcome</p>
            <h1 className="hero-title">{welcomeName}</h1>
            <div className="hero-sub">{displayName}</div>
            {selectedAccount?.status ? <span className="badge badge-outline mt-2">{selectedAccount.status}</span> : null}
          </div>
          <div className="stage-actions">
            <button
              type="button"
              className={`btn btn-primary ${isRefreshing ? 'loading' : ''}`}
              onClick={handleRefresh}
              disabled={!selectedEiUserId || isRefreshing}
            >
              {isRefreshing ? 'Refreshing...' : 'Refresh Snapshot'}
            </button>
            <div className="text-xs opacity-70">Last sync: {formatDate(snapshot?.lastFetchedUtc ?? selectedAccount?.lastFetchedUtc)}</div>
          </div>
        </div>

        <div className="stat-grid">
          <div className="stat-tile">
            <p className="stat-label">Soul Eggs</p>
            <p className="stat-value">{formatSoulEggs(snapshot?.soulEggs)}</p>
          </div>
          <div className="stat-tile">
            <p className="stat-label">Prophecy Eggs</p>
            <p className="stat-value">{formatNumber(snapshot?.eggsOfProphecy, 0)}</p>
          </div>
          <div className="stat-tile">
            <p className="stat-label">Truth Eggs</p>
            <p className="stat-value">{formatNumber(snapshot?.truthEggs, 0)}</p>
          </div>
          <div className="stat-tile">
            <p className="stat-label">Golden Eggs</p>
            <p className="stat-value">{formatNumber(snapshot?.goldenEggsBalance, 0)}</p>
          </div>
          <div className="stat-tile">
            <p className="stat-label">MER</p>
            <p className="stat-value">{formatNumber(snapshot?.mer, 2)}</p>
          </div>
          <div className="stat-tile">
            <p className="stat-label">JER</p>
            <p className="stat-value">{formatNumber(snapshot?.jer, 2)}</p>
          </div>
          <div className="stat-tile">
            <p className="stat-label">EB%</p>
            <p className="stat-value">{formatNumber(snapshot?.eb, 2)}</p>
          </div>
        </div>

        {refreshError ? <p className="text-error text-sm mt-4">{refreshError}</p> : null}
      </section>
    </div>
  )
}

export default MainPage
