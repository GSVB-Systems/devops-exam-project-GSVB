export type LeaderboardEntry = {
  eiUserId: string
  status: string
  userName?: string | null
  eb?: number | null
  soulEggs?: number | null
  eggsOfProphecy?: number | null
  mer?: number | null
  jer?: number | null
  lastFetchedUtc: string
}

export type ApiError = {
  message: string
  status?: number
}

const ensureOk = async (response: Response) => {
  if (!response.ok) {
    const message = await response.text()
    const error: ApiError = {
      message: message || `Request failed with status ${response.status}`,
      status: response.status
    }
    throw error
  }
}

const getLeaderboardEntries = async (token: string): Promise<LeaderboardEntry[]> => {
  const response = await fetch('/api/leaderboards', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  await ensureOk(response)
  return (await response.json()) as LeaderboardEntry[]
}

export const leaderboardApi = {
  getLeaderboardEntries
}
