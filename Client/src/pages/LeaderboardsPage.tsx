import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import LeaderboardChart from '../components/LeaderboardChart'
import { leaderboardApi, type LeaderboardEntry } from '../api/leaderboardApi'

const LeaderboardsPage = () => {
  const [entries, setEntries] = useState<LeaderboardEntry[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const load = async () => {
      const token = localStorage.getItem('accessToken')
      if (!token) {
        setError('Access token is missing in localStorage (accessToken).')
        return
      }

      setIsLoading(true)
      setError(null)

      try {
        const result = await leaderboardApi.getLeaderboardEntries(token)
        setEntries(result)
      } catch (err) {
        const message = err instanceof Error ? err.message : 'Failed to load leaderboards.'
        setError(message)
      } finally {
        setIsLoading(false)
      }
    }

    void load()
  }, [])

  const totalAccounts = entries.length

  const pointsEbSoul = useMemo(() => {
    return entries
      .filter((entry) => (entry.eb ?? 0) > 0 && (entry.soulEggs ?? 0) > 0)
      .map((entry) => [Number(entry.eb), Number(entry.soulEggs)] as [number, number])
  }, [entries])

  const pointsEbProphecy = useMemo(() => {
    return entries
      .filter((entry) => (entry.eb ?? 0) > 0 && (entry.eggsOfProphecy ?? 0) > 0)
      .map((entry) => [Number(entry.eb), Number(entry.eggsOfProphecy)] as [number, number])
  }, [entries])

  const pointsEbMer = useMemo(() => {
    return entries
      .filter((entry) => (entry.eb ?? 0) > 0 && (entry.mer ?? 0) > 0)
      .map((entry) => [Number(entry.eb), Number(entry.mer)] as [number, number])
  }, [entries])

  const pointsEbJer = useMemo(() => {
    return entries
      .filter((entry) => (entry.eb ?? 0) > 0 && (entry.jer ?? 0) > 0)
      .map((entry) => [Number(entry.eb), Number(entry.jer)] as [number, number])
  }, [entries])

  return (
    <div className="leaderboard-stage">
      <header className="shell-panel rise-in leaderboard-header">
        <div>
          <p className="eyebrow">Global leaderboards</p>
          <h1 className="hero-title">Leaderboard Explorer</h1>
          <p className="hero-sub">Zoom and pan to inspect clusters across EB, soul eggs, prophecy eggs, MER, and JER.</p>
        </div>
        <div className="leaderboard-header__actions">
          <Link to="/" className="btn btn-outline">
            Back to dashboard
          </Link>
          <div className="leaderboard-meta">
            <span className="badge badge-outline">{totalAccounts} accounts</span>
            <span className="leaderboard-meta__text">Log axes, scroll or drag to zoom</span>
          </div>
        </div>
      </header>

      {isLoading ? <p className="text-sm opacity-70">Loading leaderboard data...</p> : null}
      {error ? <p className="text-error text-sm">{error}</p> : null}

      <section className="leaderboard-grid">
        <LeaderboardChart
          title="EB vs Soul Eggs"
          subtitle={`${pointsEbSoul.length} points`}
          xLabel="EB (log)"
          yLabel="Soul Eggs (log)"
          points={pointsEbSoul}
        />
        <LeaderboardChart
          title="EB vs Prophecy Eggs"
          subtitle={`${pointsEbProphecy.length} points`}
          xLabel="EB (log)"
          yLabel="Prophecy Eggs (log)"
          points={pointsEbProphecy}
          accent="#7dd9c5"
        />
        <LeaderboardChart
          title="EB vs MER"
          subtitle={`${pointsEbMer.length} points`}
          xLabel="EB (log)"
          yLabel="MER (log)"
          points={pointsEbMer}
          accent="#f38f6b"
        />
        <LeaderboardChart
          title="EB vs JER"
          subtitle={`${pointsEbJer.length} points`}
          xLabel="EB (log)"
          yLabel="JER (log)"
          points={pointsEbJer}
          accent="#9f8bff"
        />
      </section>
    </div>
  )
}

export default LeaderboardsPage
