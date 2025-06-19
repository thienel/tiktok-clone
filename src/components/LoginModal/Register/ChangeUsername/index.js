import { useState } from 'react'
import classNames from 'classnames/bind'
import stylesContent from '~/components/LoginModal/LoginModal.module.scss'
import styles from './ChangeUsername.module.scss'
import Username from '~/components/LoginModal/InputForms/Username'
import SubmitButton from '~/components/LoginModal/SubmitButton'
import { useAuth, useUsersAPI } from '~/hooks'

const cxContent = classNames.bind(stylesContent)
const cx = classNames.bind(styles)

function ChangeUsername({ email, onLogin }) {
  const [username, setUsername] = useState('')
  const [valid, setValid] = useState(false)
  const { isLoggingIn } = useAuth()
  const { changeUsername, isChangingUsername } = useUsersAPI()

  const handleChangeUsername = async () => {
    if (!valid) return
    try {
      console.log('changeUsername, email: ', email, ', username: ', username)
      const result = await changeUsername(email, username, 'email')
      if (result.success) {
        await onLogin()
      }
    } catch (err) {
      console.log('Error during change username: ', err)
    }
  }

  return (
    <div className={cxContent('contentWrapper')}>
      <h2 className={cxContent('contentTitle')}>Sign up</h2>
      <Username username={username} setUsername={setUsername} setValid={(value) => setValid(value)} />
      <SubmitButton
        disabled={!valid}
        content={'Sign up'}
        loading={isChangingUsername}
        onClick={handleChangeUsername}
        className={cx('buttonGroup')}
      />
      <SubmitButton
        content={'Skip'}
        loading={isLoggingIn}
        onClick={onLogin}
        className={cx('skipButton', 'buttonGroup')}
      />
    </div>
  )
}

export default ChangeUsername
