import { Fragment } from 'react'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { publicRoutes } from '~/routes'
import { DefaultLayout } from '~/components/Layout'

function App() {
  return (
    <Router>
      <Routes>
        {publicRoutes.map((route, index) => {
          var Layout = DefaultLayout
          if (route.layout) {
            Layout = route.layout
          } else if (route.layout === null) {
            Layout = Fragment
          }
          const Page = route.component
          return (
            <Route
              key={index}
              path={route.path}
              element={
                <Layout>
                  <Page />
                </Layout>
              }
            />
          )
        })}
      </Routes>
    </Router>
  )
}

export default App
