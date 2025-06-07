import { useState, useEffect } from 'react'
import classNames from 'classnames/bind'
import styles from './Register.module.scss'
import { useAuth } from '~/hooks'
import { EmailInput, PasswordInput, VerificationCode, BirthdaySelector } from '../InputForms'
import SubmitButton from '../SubmitButton'
import ChangeUsername from './ChangeUsername'

const cx = classNames.bind(styles)

function Register({ open }) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [verificationCode, setVerificationCode] = useState('')
  const [birthDate, setBirthDate] = useState('')
  const [allFieldValid, setAllFieldValid] = useState({
    birthday: false,
    email: false,
    password: false,
    verificationCode: false,
  })
  const [canNext, setCanNext] = useState(false)
  const [error, setError] = useState('')
  const [countdown, setCountdown] = useState(0)
  const [emailSent, setEmailSent] = useState(false)

  const { sendEmailVerification, loading, LOADING_TYPE, register } = useAuth()

  const sendEmailButtonActive = !!birthDate && !!email
  const handleSendVerification = async () => {
    if (!allFieldValid.birthday || !allFieldValid.email) return

    try {
      const result = await sendEmailVerification(email)
      setError(result.errorCode ? result.errorCode : '')
      if (result.success) {
        setEmailSent(true)
        setCountdown(60)
      }
    } catch (err) {
      console.error('Error sending verification:', err)
    }
  }

  const handleRegister = async () => {
    if (!canNext) return

    try {
      const result = await register(email, password, birthDate, verificationCode)
      console.log(result)
      setError(result.errorCode ? result.errorCode : '')
    } catch (err) {
      console.log('Error during register: ', err)
    }
  }

  useEffect(() => {
    setCanNext(
      allFieldValid.birthday && allFieldValid.email && allFieldValid.password && allFieldValid.verificationCode,
    )
  }, [allFieldValid])

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

  return (
    <>
      <div className={cx('wrapper', {})}>
        <h2 className={cx('title')}>Sign up</h2>

        <BirthdaySelector
          setBirthDate={setBirthDate}
          setValid={(value) => setAllFieldValid((prev) => ({ ...prev, birthday: value }))}
          errorCode={error}
        />

        <div className={cx('title-method')}>Email</div>
        <EmailInput
          email={email}
          setEmail={setEmail}
          setValid={(value) => setAllFieldValid((prev) => ({ ...prev, email: value }))}
          errorCode={error}
        />
        <PasswordInput
          password={password}
          setPassword={setPassword}
          setValid={(value) => setAllFieldValid((prev) => ({ ...prev, password: value }))}
        />
        <VerificationCode
          verificationCode={verificationCode}
          setVerificationCode={setVerificationCode}
          setValid={(value) => setAllFieldValid((prev) => ({ ...prev, verificationCode: value }))}
          onSendVerification={handleSendVerification}
          sendButtonActive={sendEmailButtonActive}
          loading={loading === LOADING_TYPE.SEND_EMAIL}
          errorCode={error}
          onResetErrorCode={() => setError('')}
          countdown={countdown}
        />
        <SubmitButton
          disabled={!canNext}
          loading={loading === LOADING_TYPE.REGISTER}
          content={'Next'}
          onClick={handleRegister}
        />
      </div>
      <ChangeUsername open={true} />
    </>
  )
}

export default Register
