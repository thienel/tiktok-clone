import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import reportWebVitals from './reportWebVitals'
import GlobalStyles from './styles'
import { ThemeProvider } from './context/ThemeContext'

const root = ReactDOM.createRoot(document.getElementById('root'))
root.render(
  // <React.StrictMode>
  <ThemeProvider>
    <GlobalStyles>
      <App />
    </GlobalStyles>
  </ThemeProvider>,
  // </React.StrictMode>,
)

reportWebVitals()
