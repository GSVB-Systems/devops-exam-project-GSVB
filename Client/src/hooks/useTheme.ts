import { useEffect, useState } from 'react'

export type ThemeName = 'default' | 'alternate'

const THEME_STORAGE_KEY = 'ui-theme'

const isThemeName = (value: string | null): value is ThemeName => {
  return value === 'default' || value === 'alternate'
}



export const useTheme = () => {
  const [theme, setTheme] = useState<ThemeName>(() => {
    const storedTheme = localStorage.getItem(THEME_STORAGE_KEY)
    return isThemeName(storedTheme) ? storedTheme : 'default'
  })

  useEffect(() => {
    document.documentElement.dataset.theme = theme
    localStorage.setItem(THEME_STORAGE_KEY, theme)
  }, [theme])

  const toggleTheme = () => {
    setTheme((previousTheme) => (previousTheme === 'default' ? 'alternate' : 'default'))
  }

  return { theme, setTheme, toggleTheme }
}

