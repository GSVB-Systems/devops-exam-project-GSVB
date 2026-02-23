import { useEffect, useMemo, useRef } from 'react'
import uPlot from 'uplot'
import 'uplot/dist/uPlot.min.css'

type LeaderboardChartProps = {
  title: string
  subtitle?: string
  xLabel: string
  yLabel: string
  points: Array<[number, number]>
  accent?: string
}

const defaultAccent = '#f5d072'

const formatMagnitude = (value: number) => {
  if (!Number.isFinite(value)) return ''
  if (value === 0) return '0'
  const absValue = Math.abs(value)
  if (absValue < 1e3) return value.toFixed(0)
  const units = [
    { threshold: 1e3, suffix: 'K' },
    { threshold: 1e6, suffix: 'M' },
    { threshold: 1e9, suffix: 'B' },
    { threshold: 1e12, suffix: 'T' },
    { threshold: 1e15, suffix: 'q' },
    { threshold: 1e18, suffix: 'Q' },
    { threshold: 1e21, suffix: 's' },
    { threshold: 1e24, suffix: 'S' },
    { threshold: 1e27, suffix: 'o' },
    { threshold: 1e30, suffix: 'N' }
  ]
  const unitIndex = Math.min(units.length - 1, Math.max(0, Math.floor(Math.log10(absValue) / 3) - 1))
  const unit = units[unitIndex]
  return `${(value / unit.threshold).toFixed(1)}${unit.suffix}`
}

const LeaderboardChart = ({ title, subtitle, xLabel, yLabel, points, accent }: LeaderboardChartProps) => {
  const containerRef = useRef<HTMLDivElement | null>(null)
  const plotRef = useRef<uPlot | null>(null)

  const data = useMemo(() => {
    const filtered = points.filter(([x, y]) => x > 0 && y > 0)
    const sorted = [...filtered].sort((a, b) => a[0] - b[0])
    const xValues = sorted.map(([x]) => x)
    const yValues = sorted.map(([, y]) => y)
    return [xValues, yValues] as [number[], number[]]
  }, [points])

  useEffect(() => {
    if (!containerRef.current) return

    if (plotRef.current) {
      plotRef.current.setData(data)
      return
    }

    const accentColor = accent ?? defaultAccent

    const { width, height } = containerRef.current.getBoundingClientRect()
    const plot = new uPlot(
      {
        width: Math.max(240, Math.floor(width)),
        height: Math.max(260, Math.floor(height)),
        title,
        scales: {
          x: { time: false, distr: 3, log: 10 },
          y: { distr: 3, log: 10 }
        },
        axes: [
          {
            label: xLabel,
            stroke: '#bfb9ad',
            grid: { stroke: 'rgba(255, 255, 255, 0.08)' },
            values: (_u, splits) => splits.map(formatMagnitude)
          },
          {
            label: yLabel,
            stroke: '#bfb9ad',
            grid: { stroke: 'rgba(255, 255, 255, 0.08)' },
            values: (_u, splits) => splits.map(formatMagnitude)
          }
        ],
        cursor: {
          drag: { setScale: true }
        },
        series: [
          {},
          {
            label: yLabel,
            width: 0,
            points: {
              size: 2,
              stroke: accentColor,
              fill: accentColor
            },
            stroke: accentColor
          }
        ]
      },
      data,
      containerRef.current
    )

    plotRef.current = plot

    return () => {
      plot.destroy()
      plotRef.current = null
    }
  }, [accent, data, title, xLabel, yLabel])

  useEffect(() => {
    const container = containerRef.current
    const plot = plotRef.current
    if (!container || !plot) return

    const resizeObserver = new ResizeObserver(() => {
      if (!containerRef.current || !plotRef.current) return
      const { width, height } = containerRef.current.getBoundingClientRect()
      plotRef.current.setSize({ width: Math.max(240, Math.floor(width)), height: Math.max(240, Math.floor(height)) })
    })

    resizeObserver.observe(container)

    return () => resizeObserver.disconnect()
  }, [])

  return (
    <div className="leaderboard-card shell-panel">
      <div className="leaderboard-card__header">
        <div>
          <p className="eyebrow">Leaderboard graph</p>
          <h3 className="panel-title">{title}</h3>
        </div>
        {subtitle ? <p className="leaderboard-card__meta">{subtitle}</p> : null}
      </div>
      <div className="leaderboard-chart" ref={containerRef} />
    </div>
  )
}

export default LeaderboardChart
