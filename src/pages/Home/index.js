import { useState } from 'react'
import FixedTop from '~/components/FixedTop'
import LoginModal from '~/components/LoginModal'

function Home() {
  const [login, setLogin] = useState(false)
  return (
    <div>
      <h2 style={{ textAlign: 'center' }}>Home page</h2>
      <FixedTop
        onLogin={() => {
          setLogin(true)
        }}
      />
      <LoginModal onClose={() => setLogin(false)} isOpen={login} />
    </div>
  )
}

export default Home
