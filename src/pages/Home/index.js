import { useState } from 'react'
import FixedTop from '~/components/FixedTop'
import LoginModal from '~/components/LoginModal'
import { MainLayout } from '~/layouts'

function Home() {
  const [login, setLogin] = useState(false)
  return (
    <MainLayout>
      <div>
        <h2 style={{ textAlign: 'center' }}>Home page</h2>
        <FixedTop
          onLogin={() => {
            setLogin(true)
          }}
        />
        <LoginModal onClose={() => setLogin(false)} isOpen={login} />
      </div>
    </MainLayout>
  )
}

export default Home
