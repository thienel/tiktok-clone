import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { publicRoutes } from '~/routes'
import ErrorBoundary from '~/components/ErrorBoundary'
import { AuthProvider } from '~/context/AuthContext'
import { LoadingProvider } from '~/context/LoadingContext'

function App() {
  return (
    <ErrorBoundary>
      <LoadingProvider>
        <AuthProvider>
          <Router>
            <Routes>
              {publicRoutes.map((route, index) => {
                const Page = route.component
                return <Route key={index} path={route.path} element={<Page />} />
              })}
            </Routes>
          </Router>
        </AuthProvider>
      </LoadingProvider>
    </ErrorBoundary>
  )
}

export default App
