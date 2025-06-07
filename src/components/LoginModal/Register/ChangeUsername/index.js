import classNames from 'classnames/bind'
import styles from './ChangeUsername.module.scss'
import Username from '../../InputForms/Username'
import { useState } from 'react'
import SubmitButton from '../../SubmitButton'

const cx = classNames.bind(styles)

function ChangeUsername({ open }) {
  const [username, setUsername] = useState('')
  const [valid, setValid] = useState(false)

  return (
    <div className={cx('wrapper', { open })}>
      <h2 className={cx('title')}>Sign up</h2>
      <Username username={username} setUsername={setUsername} setValid={(value) => setValid(value)} />
      <SubmitButton disabled={!valid} content={'Sign up'} loading={false} onClick={() => {}} />
    </div>
  )
}

export default ChangeUsername
