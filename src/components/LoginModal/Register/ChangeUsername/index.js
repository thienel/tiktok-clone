import { useState } from 'react'
import classNames from 'classnames/bind'
import styles from '../Register.module.scss'
import Username from '../../InputForms/Username'
import SubmitButton from '../../SubmitButton'
import { useAuth } from '~/hooks'

const cx = classNames.bind(styles)

function ChangeUsername({ email, onLogin }) {
  const [username, setUsername] = useState('')
  const [valid, setValid] = useState(false)
  const { changeUsername, loading, LOADING_TYPE } = useAuth()

  const handleChangeUsername = async () => {
    if (!valid) return
    try {
      const result = await changeUsername(email, username)
      if (result.success) {
        await onLogin()
      }
    } catch (err) {
      console.log('Error during change username: ', err)
    }
  }

  return (
    <div className={cx('wrapper')}>
      <h2 className={cx('title')}>Sign up</h2>
      <Username username={username} setUsername={setUsername} setValid={(value) => setValid(value)} />
      <SubmitButton
        disabled={!valid}
        content={'Sign up'}
        loading={loading === LOADING_TYPE.CHANGE_USERNAME}
        onClick={handleChangeUsername}
        className={cx('buttonGroup')}
      />
      <SubmitButton
        content={'Skip'}
        loading={loading === LOADING_TYPE.LOGIN}
        onClick={onLogin}
        className={cx('skipButton', 'buttonGroup')}
      />
    </div>
  )
}

export default ChangeUsername
