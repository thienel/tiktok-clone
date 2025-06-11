import { useState } from 'react'
import FixedTop from '~/components/FixedTop'
import LoginModal from '~/components/LoginModal'
import { MainLayout } from '~/layouts'
import { useAuth } from '~/hooks'
import FeedVideo from '~/components/FeedVideo'

function Home() {
  const [login, setLogin] = useState(false)
  const { isAuthenticated } = useAuth()
  return (
    <MainLayout>
      <FeedVideo />
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
    </MainLayout>
  )
}

export default Home
