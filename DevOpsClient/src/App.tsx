import { useState } from 'react'
import './App.css'

function App() {
  const [userId, setUserId] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [result, setResult] = useState<Record<string, unknown> | null>(null)

  const handleRefresh = async () => {
    if (!userId.trim()) {
      setError('User ID is required.')
      return
    }

    setIsLoading(true)
    setError(null)
    setResult(null)

    try {
      const response = await fetch(`/api/egg-snapshots/${encodeURIComponent(userId)}/refresh`, {
        method: 'POST'
      })

      if (!response.ok) {
        const message = await response.text()
        throw new Error(message || `Request failed with status ${response.status}`)
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
    <div className="card">
      <h1>Egg Snapshot Refresh</h1>
      <label htmlFor="userId">User ID</label>
      <input
        id="userId"
        type="text"
        value={userId}
        onChange={(event) => setUserId(event.target.value)}
        placeholder="Enter user id"
        autoComplete="off"
      />
      <button type="button" onClick={handleRefresh} disabled={isLoading}>
        {isLoading ? 'Refreshing...' : 'Refresh Snapshot'}
      </button>
      {error ? <p className="error">{error}</p> : null}
      {result ? (
        <pre className="result">{JSON.stringify(result, null, 2)}</pre>
      ) : null}
    </div>
  )
}

export default App
