import { useState, createContext, useEffect } from 'react'
import { themeID } from '~/constants/drawer'

export const ThemeContext = createContext()

export const ThemeProvider = ({ children }) => {
  const getDeviceTheme = () => {
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? themeID.DARK : themeID.LIGHT
  }

  const [themeSetting, setThemeSetting] = useState(() => {
    return localStorage.getItem('themeSetting') || themeID.DEVICE
  })

  const [resolvedTheme, setResolvedTheme] = useState(() => {
    return themeSetting === themeID.DEVICE ? getDeviceTheme() : themeSetting
  })

  useEffect(() => {
    const updateResolvedTheme = () => {
      const nextTheme = themeSetting === themeID.DEVICE ? getDeviceTheme() : themeSetting
      setResolvedTheme(nextTheme)
      document.documentElement.setAttribute('data-theme', nextTheme)
    }

    updateResolvedTheme()
    localStorage.setItem('themeSetting', themeSetting)

    let mql
    if (themeSetting === themeID.DEVICE) {
      mql = window.matchMedia('(prefers-color-scheme: dark)')
      mql.addEventListener('change', updateResolvedTheme)
    }

    return () => {
      mql?.removeEventListener('change', updateResolvedTheme)
    }
  }, [themeSetting])

  const handleSetTheme = (mode) => {
    if (mode !== themeSetting && Object.values(themeID).includes(mode)) {
      setThemeSetting(mode)
    }
  }

  return (
    <ThemeContext.Provider value={{ resolvedTheme, themeSetting, handleSetTheme }}>{children}</ThemeContext.Provider>
  )
}
