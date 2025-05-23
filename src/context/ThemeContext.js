import { useState, createContext, useEffect } from 'react'

export const ThemeContext = createContext()

export const ThemeProvider = ({ children }) => {
  const [theme, setTheme] = useState(() => {
    return localStorage.getItem('theme') || 'light'
  })

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme)
    localStorage.setItem('theme', theme)
  }, [theme])

  const handleSetTheme = (mode) => {
    if (mode !== theme) {
      if (mode === 'dark' || mode === 'light') setTheme(mode)
    }
  }

  return <ThemeContext.Provider value={{ theme, handleSetTheme }}>{children}</ThemeContext.Provider>
}
