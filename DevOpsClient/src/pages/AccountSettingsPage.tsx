import { useEffect, useState } from 'react'
import { userApi, type EggAccount, type UserProfile } from '../api/userApi'

const AccountSettingsPage = () => {
  const [profile, setProfile] = useState<UserProfile | null>(null)
  const [eggAccounts, setEggAccounts] = useState<EggAccount[]>([])
  const [username, setUsername] = useState('')
  const [discordUsername, setDiscordUsername] = useState('')
  const [eiUserId, setEiUserId] = useState('')
  const [status, setStatus] = useState<'Main' | 'Alt'>('Alt')
  const [isSaving, setIsSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const token = localStorage.getItem('accessToken')

  const loadData = async () => {
    if (!token) {
      setError('Access token is missing in localStorage (accessToken).')
      return
    }

    setError(null)
    try {
      const [user, accounts] = await Promise.all([userApi.getCurrentUser(token), userApi.getEggAccounts(token)])
      setProfile(user)
      setUsername(user.username ?? '')
      setDiscordUsername(user.discordUsername ?? '')
      setEggAccounts(accounts)
      const main = accounts.find((account) => account.status === 'Main')
      if (main) {
        setStatus('Alt')
      }
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to load account settings.'
      setError(message)
    }
  }

  useEffect(() => {
    void loadData()
  }, [])

  const handleSaveProfile = async () => {
    if (!token) {
      setError('Access token is missing in localStorage (accessToken).')
      return
    }

    if (!username.trim()) {
      setError('Username is required.')
      return
    }

    setIsSaving(true)
    setError(null)

    try {
      const updated = await userApi.updateCurrentUser(token, {
        username: username.trim(),
        discordUsername: discordUsername.trim() ? discordUsername.trim() : null
      })
      setProfile(updated)
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to save account settings.'
      setError(message)
    } finally {
      setIsSaving(false)
    }
  }

  const handleAddEggAccount = async () => {
    if (!token) {
      setError('Access token is missing in localStorage (accessToken).')
      return
    }

    if (!eiUserId.trim()) {
      setError('Ei User ID is required.')
      return
    }

    setIsSaving(true)
    setError(null)

    try {
      await userApi.createEggAccount(token, {
        eiUserId: eiUserId.trim(),
        status
      })
      setEiUserId('')
      setStatus('Alt')
      await loadData()
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to add egg account.'
      setError(message)
    } finally {
      setIsSaving(false)
    }
  }

  const handleMakeMain = async (account: EggAccount) => {
    if (!token) {
      setError('Access token is missing in localStorage (accessToken).')
      return
    }

    setIsSaving(true)
    setError(null)

    try {
      await userApi.updateEggAccount(token, account.id, { status: 'Main' })
      await loadData()
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to update egg account.'
      setError(message)
    } finally {
      setIsSaving(false)
    }
  }

  const handleDelete = async (account: EggAccount) => {
    if (!token) {
      setError('Access token is missing in localStorage (accessToken).')
      return
    }

    setIsSaving(true)
    setError(null)

    try {
      await userApi.deleteEggAccount(token, account.id)
      await loadData()
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to delete egg account.'
      setError(message)
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <div className="space-y-6">
      <div className="card w-full max-w-2xl mx-auto bg-base-100 shadow-md">
        <div className="card-body">
          <h2 className="card-title">Account Settings</h2>

          <div className="space-y-4">
            <div>
              <label htmlFor="username" className="label">
                <span className="label-text">Username</span>
              </label>
              <input
                id="username"
                type="text"
                value={username}
                onChange={(event) => setUsername(event.target.value)}
                className="input input-bordered w-full"
              />
            </div>

            <div>
              <label htmlFor="discordUsername" className="label">
                <span className="label-text">Discord Username</span>
              </label>
              <input
                id="discordUsername"
                type="text"
                value={discordUsername}
                onChange={(event) => setDiscordUsername(event.target.value)}
                className="input input-bordered w-full"
              />
            </div>

            <div className="text-sm opacity-70">Email: {profile?.email ?? 'Loading...'}</div>

            <button type="button" className="btn btn-primary" onClick={handleSaveProfile} disabled={isSaving}>
              {isSaving ? 'Saving...' : 'Save Profile'}
            </button>
          </div>
        </div>
      </div>

      <div className="card w-full max-w-2xl mx-auto bg-base-100 shadow-md">
        <div className="card-body">
          <h2 className="card-title">Linked Egg Accounts</h2>

          <div className="space-y-4">
            <div className="grid gap-3 md:grid-cols-3">
              <input
                type="text"
                value={eiUserId}
                onChange={(event) => setEiUserId(event.target.value)}
                placeholder="Ei User ID"
                className="input input-bordered w-full"
              />
              <select
                className="select select-bordered w-full"
                value={status}
                onChange={(event) => setStatus(event.target.value as 'Main' | 'Alt')}
              >
                <option value="Main">Main</option>
                <option value="Alt">Alt</option>
              </select>
              <button type="button" className="btn btn-primary" onClick={handleAddEggAccount} disabled={isSaving}>
                Add Account
              </button>
            </div>

            <div className="divider">Existing</div>

            {eggAccounts.length === 0 ? (
              <p className="text-sm opacity-70">No linked accounts yet.</p>
            ) : (
              <div className="space-y-3">
                {eggAccounts.map((account) => (
                  <div key={account.id} className="flex flex-col gap-2 rounded-lg border border-base-300 p-3">
                    <div className="flex flex-wrap items-center justify-between gap-2">
                      <div>
                        <div className="font-semibold">
                          {account.userName ? account.userName : account.eiUserId}
                        </div>
                        <div className="text-xs opacity-70">{account.eiUserId}</div>
                      </div>
                      <span className="badge badge-outline">{account.status}</span>
                    </div>
                    <div className="flex flex-wrap gap-2">
                      {account.status !== 'Main' ? (
                        <button type="button" className="btn btn-sm" onClick={() => handleMakeMain(account)}>
                          Make Main
                        </button>
                      ) : null}
                      <button type="button" className="btn btn-sm btn-outline" onClick={() => handleDelete(account)}>
                        Remove
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>

      {error ? <p className="text-error text-center">{error}</p> : null}
    </div>
  )
}

export default AccountSettingsPage
