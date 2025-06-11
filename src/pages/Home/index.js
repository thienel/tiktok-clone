import { useState } from 'react'
import FixedTop from '~/components/FixedTop'
import LoginModal from '~/components/LoginModal'
import { MainLayout } from '~/layouts'
import { useAuth } from '~/hooks'

function Home() {
  const [login, setLogin] = useState(false)
  const { isAuthenticated } = useAuth()
  return (
    <MainLayout>
      <div>
        <h2 style={{ textAlign: 'center' }}>Home page</h2>
        {!isAuthenticated && (
          <>
            <FixedTop
              onLogin={() => {
                setLogin(true)
              }}
            />
            <LoginModal onClose={() => setLogin(false)} isOpen={login} />
          </>
        )}
      </div>
    </MainLayout>
  )
}

export default Home
