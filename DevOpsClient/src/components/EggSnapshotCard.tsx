import React, { useState } from 'react'
import '../App.css'

type EggSnapshotCardProps = {
  eiUserId: string | null
}

const EggSnapshotCard: React.FC<EggSnapshotCardProps> = ({ eiUserId }) => {
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [result, setResult] = useState<Record<string, unknown> | null>(null)

  const handleRefresh = async () => {
    if (!eiUserId) {
      setError('Select an egg account first.')
      return
    }

    const token = localStorage.getItem('accessToken')
    if (!token) {
      setError('Access token is missing in localStorage (accessToken).')
      return
    }

    setIsLoading(true)
    setError(null)
    setResult(null)

    try {
      const response = await fetch(`/api/egg-accounts/refresh/${encodeURIComponent(eiUserId)}`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${token}`
        }
      })

      if (!response.ok) {
        const message = await response.text()
        setError(message || `Request failed with status ${response.status}`)
        return
      }

      const data = (await response.json()) as Record<string, unknown>
      setResult(data)
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Request failed.'
      setError(message)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="card w-full max-w-xl mx-auto bg-base-100 shadow-md">
      <div className="card-body">
        <h2 className="card-title">Egg Snapshot Refresh</h2>

        <div className="form-control mt-4">
          <button
            type="button"
            onClick={handleRefresh}
            disabled={isLoading || !eiUserId}
            className={`btn btn-primary ${isLoading ? 'loading' : ''}`}
          >
            {isLoading ? 'Refreshing...' : 'Refresh Snapshot'}
          </button>
        </div>

        {!eiUserId ? <p className="text-sm opacity-70 mt-2">Choose an egg account to refresh.</p> : null}

        {error ? <p className="text-error mt-2">{error}</p> : null}

        {result ? (
          <div className="mt-4">
            <pre className="result max-h-96 overflow-auto p-2 bg-base-200 rounded">{JSON.stringify(result, null, 2)}</pre>
          </div>
        ) : null}
      </div>
    </div>
  )
}

export default EggSnapshotCard
