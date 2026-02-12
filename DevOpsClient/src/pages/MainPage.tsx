import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { userApi, type EggAccount, type EggAccountRefresh, type UserProfile } from '../api/userApi'

const MainPage = () => {
  const selectedAccountStorageKey = 'selectedEiUserId'
  const snapshotStorageKey = 'eggSnapshotsByUserId'
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
        const storedEiUserId = localStorage.getItem(selectedAccountStorageKey)
        const storedAccount = storedEiUserId
          ? accounts.find((account) => account.eiUserId === storedEiUserId)
          : null
        const defaultEiUserId = storedAccount?.eiUserId ?? mainAccount?.eiUserId ?? accounts[0]?.eiUserId ?? null
        setSelectedEiUserId(defaultEiUserId)
      } catch (err) {
        const message = err instanceof Error ? err.message : 'Failed to load egg accounts.'
        setError(message)
      }
    }

    void loadAccounts()
  }, [])

  const loadSnapshotCache = () => {
    const raw = localStorage.getItem(snapshotStorageKey)
    if (!raw) return {}
    try {
      const parsed = JSON.parse(raw)
      return parsed && typeof parsed === 'object' ? parsed : {}
    } catch {
      return {}
    }
  }

  const saveSnapshotCache = (cache: Record<string, EggAccountRefresh>) => {
    localStorage.setItem(snapshotStorageKey, JSON.stringify(cache))
  }

  useEffect(() => {
    if (!selectedEiUserId) {
      setSnapshot(null)
      setRefreshError(null)
      return
    }

    const cachedSnapshots = loadSnapshotCache()
    const cachedSnapshot = cachedSnapshots[selectedEiUserId] ?? null
    setSnapshot(cachedSnapshot)
    setRefreshError(null)
  }, [selectedEiUserId])

  useEffect(() => {
    if (!selectedEiUserId) return
    localStorage.setItem(selectedAccountStorageKey, selectedEiUserId)
  }, [selectedEiUserId])

  const selectedAccount = useMemo(
    () => eggAccounts.find((account) => account.eiUserId === selectedEiUserId) ?? null,
    [eggAccounts, selectedEiUserId]
  )

  const formatNumber = (value: number | null | undefined, digits = 2) => {
    if (value === null || value === undefined) return '--'
    return value.toLocaleString(undefined, { maximumFractionDigits: digits })
  }

  const magnitudeUnits = [
    { threshold: 1e3, suffix: 'K' },
    { threshold: 1e6, suffix: 'M' },
    { threshold: 1e9, suffix: 'B' },
    { threshold: 1e12, suffix: 'T' },
    { threshold: 1e15, suffix: 'q' },
    { threshold: 1e18, suffix: 'Q' },
    { threshold: 1e21, suffix: 's' },
    { threshold: 1e24, suffix: 'S' },
    { threshold: 1e27, suffix: 'o' },
    { threshold: 1e30, suffix: 'N' },
    { threshold: 1e33, suffix: 'd' },
    { threshold: 1e36, suffix: 'U' },
    { threshold: 1e39, suffix: 'D' },
    { threshold: 1e42, suffix: 'Td' },
    { threshold: 1e45, suffix: 'qd' },
    { threshold: 1e48, suffix: 'Qd' },
    { threshold: 1e51, suffix: 'sd' },
    { threshold: 1e54, suffix: 'Sd' },
    { threshold: 1e57, suffix: 'Od' },
    { threshold: 1e60, suffix: 'Nd' },
    { threshold: 1e63, suffix: 'V' }
  ]

  const formatMagnitude = (
    value: number | null | undefined,
    digits = 3,
    smallDigits = digits
  ) => {
    if (value === null || value === undefined) return '--'

    const absValue = Math.abs(value)
    if (absValue < 1e3) {
      return formatNumber(value, smallDigits)
    }

    const unitIndex = Math.min(
      magnitudeUnits.length - 1,
      Math.max(0, Math.floor(Math.log10(absValue) / 3) - 1)
    )
    const unit = magnitudeUnits[unitIndex]
    return `${(value / unit.threshold).toFixed(digits)}${unit.suffix}`
  }

  const formatSoulEggs = (value: number | null | undefined) => formatMagnitude(value, 3, 0)

  const formatEb = (value: number | null | undefined) => {
    const formatted = formatMagnitude(value, 3, 2)
    return formatted === '--' ? formatted : `${formatted}%`
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
      const cachedSnapshots = loadSnapshotCache()
      cachedSnapshots[selectedEiUserId] = data
      saveSnapshotCache(cachedSnapshots)
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
          <div className="shell-actions">
            <Link to="/leaderboards" className="btn btn-xs btn-outline">
              Leaderboards
            </Link>
            <Link to="/settings" className="btn btn-xs btn-outline">
              Settings
            </Link>
          </div>
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
            <p className="stat-value">{formatEb(snapshot?.eb)}</p>
          </div>
        </div>

        {refreshError ? <p className="text-error text-sm mt-4">{refreshError}</p> : null}
      </section>
    </div>
  )
}

export default MainPage
