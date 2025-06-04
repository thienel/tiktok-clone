import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import reportWebVitals from './reportWebVitals'
import GlobalStyles from './styles'
import { ThemeProvider } from './context/ThemeContext'
import { AuthProvider } from './context/AuthContext'

const root = ReactDOM.createRoot(document.getElementById('root'))
root.render(
  // <React.StrictMode>
  <ThemeProvider>
    <GlobalStyles>
      <AuthProvider>
        <App />
      </AuthProvider>
    </GlobalStyles>
  </ThemeProvider>,
  // </React.StrictMode>,
)

reportWebVitals()
