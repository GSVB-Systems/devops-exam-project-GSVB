import { useState } from 'react'
import './App.css'
import Login from './components/Login'

function App() {
  const [eiUserId, setEiUserId] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [result, setResult] = useState<Record<string, unknown> | null>(null)

  const handleRefresh = async () => {
    if (!eiUserId.trim()) {
      setError('Ei User ID is required.')
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
      const response = await fetch(`/api/egg-snapshots/refresh/${encodeURIComponent(eiUserId)}`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${token}`
        }
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
      <label htmlFor="eiUserId">Ei User ID</label>
      <input
        id="eiUserId"
        type="text"
        value={eiUserId}
        onChange={(event) => setEiUserId(event.target.value)}
        placeholder="Enter Ei User ID"
        autoComplete="off"
      />
      <button type="button" onClick={handleRefresh} disabled={isLoading}>
        {isLoading ? 'Refreshing...' : 'Refresh Snapshot'}
      </button>
      {error ? <p className="error">{error}</p> : null}
      {result ? (
        <pre className="result">{JSON.stringify(result, null, 2)}</pre>
      ) : null}
      <Login />
    </div>
  )
}

export default App
