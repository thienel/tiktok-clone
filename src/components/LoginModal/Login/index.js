import { useState } from 'react'
import classNames from 'classnames/bind'
import styles from '~/components/LoginModal/LoginModal.module.scss'
import { DefaultInput } from '~/components/LoginModal/InputForms'
import SubmitButton from '~/components/LoginModal/SubmitButton'
import { useAuth } from '~/hooks'

const cx = classNames.bind(styles)

function Login() {
  const [usernameOrEmail, setUsernameOrEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')

  const { login, loading, LOADING_TYPE } = useAuth()
  const handleLogin = async () => {
    const result = await login(usernameOrEmail, password)

    if (!result.success) setError(result.errorCode)

    if (result.success) window.location.href = '/'
  }

  return (
    <div className={cx('contentWrapper')}>
      <h2 className={cx('contentTitle')}>Log in</h2>
      <div className={cx('contentSubTitle')}>Email or username</div>
      <DefaultInput value={usernameOrEmail} setValue={setUsernameOrEmail} placeholder="Email or username" type="text" />
      <DefaultInput
        value={password}
        setValue={setPassword}
        placeholder="Password"
        type="password"
        errorCode={error}
        clearErrorCode={() => setError('')}
      />
      <SubmitButton
        disabled={!usernameOrEmail || !password || loading === LOADING_TYPE.LOGIN}
        loading={loading === LOADING_TYPE.LOGIN}
        content={'Log in'}
        onClick={handleLogin}
      />
    </div>
  )
}

export default Login
