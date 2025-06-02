import classNames from 'classnames/bind'
import styles from './Login.module.scss'

const cx = classNames.bind(styles)

function Login() {
  return (
    <div className={cx('container')}>
      <div className={cx('wrapper')}>
        <div className={cx('modal-wrapper')}>
          <div className={cx('login-wrapper')}>
            <div className={cx('home-wrapper')}>
              <h2>Log in to TikTok</h2>
              <div className={cx('login-option-wrapper')}></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Login
