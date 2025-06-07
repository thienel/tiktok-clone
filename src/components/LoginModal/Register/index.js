import { useState } from 'react'
import ChangeUsername from './ChangeUsername'
import Signup from './Signup'
import { useAuth } from '~/hooks'

function Register({ setHidePolicy }) {
  const [signUpSuccess, setSignUpSuccess] = useState(false)
  const [loginRequest, setLoginRequest] = useState({
    usernameOrEmail: '',
    password: '',
  })
  const { login } = useAuth()

  const handleSignupSuccess = (email, password) => {
    setSignUpSuccess(true)
    setHidePolicy(true)
    setLoginRequest((prev) => ({
      ...prev,
      usernameOrEmail: email,
      password: password,
    }))
  }

  const handleLogin = async () => {
    try {
      const result = await login(loginRequest.usernameOrEmail, loginRequest.password)
      if (result.success) {
        window.location.href = '/'
      }
      console.log('Login result: ', result)
    } catch (err) {
      console.log('Error during login: ', err)
    }
  }

  return (
    <>
      {!signUpSuccess && <Signup onSignupSuccess={handleSignupSuccess} />}
      {signUpSuccess && <ChangeUsername onLogin={handleLogin} />}
      {/* <ChangeUsername onLogin={handleLogin} /> */}
    </>
  )
}

export default Register
