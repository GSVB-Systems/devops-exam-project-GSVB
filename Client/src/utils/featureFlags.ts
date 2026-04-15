const parseBoolean = (value: string | undefined, defaultValue: boolean): boolean => {
  if (value === undefined) {
    return defaultValue
  }

  switch (value.trim().toLowerCase()) {
    case '1':
    case 'true':
    case 'yes':
    case 'on':
      return true
    case '0':
    case 'false':
    case 'no':
    case 'off':
      return false
    default:
      return defaultValue
  }
}

export const featureFlags = {
  leaderboards: parseBoolean(import.meta.env.VITE_FEATURE_LEADERBOARDS, true),
  admin: parseBoolean(import.meta.env.VITE_FEATURE_ADMIN, true)
}
