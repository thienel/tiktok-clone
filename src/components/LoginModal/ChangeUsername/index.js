import classNames from 'classnames/bind'
import styles from './ChangeUsername.module.scss'
import Username from '../InputForms/Username'

const cx = classNames(styles)

function ChangeUsername({ username, changeUsername }) {
  return (
    <div className={cx('wrapper')}>
      <h2 className={cx('title')}>Sign up</h2>

      <Username />
    </div>
  )
}

export default ChangeUsername
