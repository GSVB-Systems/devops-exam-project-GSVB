import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import EggSnapshotCard from '../components/EggSnapshotCard'
import { userApi, type EggAccount } from '../api/userApi'

const MainPage = () => {
  const [eggAccounts, setEggAccounts] = useState<EggAccount[]>([])
  const [selectedEiUserId, setSelectedEiUserId] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadAccounts = async () => {
      const token = localStorage.getItem('accessToken')
      if (!token) {
        setError('Access token is missing in localStorage (accessToken).')
        return
      }

      try {
        const accounts = await userApi.getEggAccounts(token)
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

  return (
    <div className="space-y-6">
      <div className="card w-full max-w-xl mx-auto bg-base-100 shadow-md">
        <div className="card-body">
          <div className="flex flex-wrap items-center justify-between gap-2">
            <h2 className="card-title">Egg Account</h2>
            <Link to="/settings" className="btn btn-sm btn-outline">
              Account Settings
            </Link>
          </div>

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

          {error ? <p className="text-error text-sm">{error}</p> : null}
        </div>
      </div>

      <EggSnapshotCard eiUserId={selectedEiUserId} />
    </div>
  )
}

export default MainPage
