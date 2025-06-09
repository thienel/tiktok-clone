import { useState, useEffect } from 'react'
import classNames from 'classnames/bind'
import styles from '~/components/LoginModal/LoginModal.module.scss'
import stylesLogin from './Login.module.scss'
import { DefaultInput, EmailInput, PasswordInput, VerificationCode } from '~/components/LoginModal/InputForms'
import SubmitButton from '~/components/LoginModal/SubmitButton'
import { useAuth } from '~/hooks'

const cx = classNames.bind(styles)
const cxLogin = classNames.bind(stylesLogin)

function Login() {
  const [usernameOrEmail, setUsernameOrEmail] = useState('')
  const [passwordLogin, setPasswordLogin] = useState('')
  const [error, setError] = useState('')
  const [type, setType] = useState('login')

  const [email, setEmail] = useState('')
  const [verificationCode, setVerificationCode] = useState('')
  const [password, setPassword] = useState('')
  const [validFields, setValidFields] = useState({
    email: false,
    verificationCode: false,
    password: false,
  })
  const [emailSent, setEmailSent] = useState(false)
  const [countdown, setCountdown] = useState(0)

  const { login, sendEmailVerification, resetPassword, loading, LOADING_TYPE } = useAuth()
  const handleLogin = async () => {
    const result = await login(usernameOrEmail, passwordLogin)

    if (!result.success) {
      setError(result.errorCode)
      return
    }

    window.location.href = '/'
  }

  const handleResetPassword = async () => {
    if (!validFields.email || !validFields.verificationCode || !validFields.password) return

    try {
      const result = await resetPassword(email, password, verificationCode)

      if (!result.success) {
        setError(result.errorCode)
        return
      }

      const resultLogin = await login(email, password)
      if (!resultLogin.success) {
        setError(resultLogin.errorCode)
        return
      }

      window.location.href = '/'
    } catch (err) {
      console.error('Reset password error: ', err)
    }
  }

  const onSendVerification = async () => {
    if (!validFields.email) return

    var result = await sendEmailVerification(email, 'reset-password')

    if (!result.success) {
      setError(result.errorCode)
      return
    }

    setEmailSent(true)
    setCountdown(60)
  }

  useEffect(() => {
    let timer
    if (countdown > 0) {
      timer = setInterval(() => {
        setCountdown((prev) => prev - 1)
      }, 1000)
    } else if (countdown === 0 && emailSent) {
      setEmailSent(false)
    }

    return () => clearInterval(timer)
  }, [emailSent, countdown])

  const isDisableLogin = () => {
    if (type === 'login') {
      return !usernameOrEmail || !passwordLogin || loading === LOADING_TYPE.LOGIN
    }
    if (type === 'reset-password') {
      return (
        !validFields.email ||
        !validFields.password ||
        !validFields.verificationCode ||
        loading === LOADING_TYPE.RESET_PASSWORD
      )
    }
  }

  return (
    <div className={cx('contentWrapper')}>
      <h2 className={cx('contentTitle')}>{type === 'login' ? 'Log in' : 'Reset password'}</h2>
      <div className={cx('contentSubTitle')}>{type === 'login' ? 'Email or username' : 'Enter email address'}</div>
      {type === 'login' && (
        <>
          <DefaultInput
            value={usernameOrEmail}
            setValue={setUsernameOrEmail}
            placeholder="Email or username"
            type="text"
          />
          <DefaultInput
            value={passwordLogin}
            setValue={setPasswordLogin}
            placeholder="Password"
            type="password"
            errorCode={error}
            clearErrorCode={() => setError('')}
          />
          <span className={cxLogin('spanLink')} onClick={() => setType('reset-password')}>
            Forgot password?
          </span>
        </>
      )}
      {type === 'reset-password' && (
        <>
          <EmailInput
            email={email}
            setEmail={setEmail}
            setValid={(value) => setValidFields((prev) => ({ ...prev, email: value }))}
            errorCode={error}
          />
          <VerificationCode
            verificationCode={verificationCode}
            setVerificationCode={setVerificationCode}
            setValid={(value) => setValidFields((prev) => ({ ...prev, verificationCode: value }))}
            errorCode={error}
            onSendVerification={onSendVerification}
            sendButtonActive={validFields.email}
            loading={loading === 'SEND_EMAIL'}
            onResetErrorCode={() => setError('')}
            countdown={countdown}
          />
          <PasswordInput
            password={password}
            setPassword={setPassword}
            setValid={(value) => setValidFields((prev) => ({ ...prev, password: value }))}
          />
        </>
      )}
      <SubmitButton
        disabled={isDisableLogin()}
        loading={loading === LOADING_TYPE.LOGIN || loading === LOADING_TYPE.RESET_PASSWORD}
        content={'Log in'}
        onClick={type === 'login' ? handleLogin : handleResetPassword}
      />
    </div>
  )
}

export default Login
