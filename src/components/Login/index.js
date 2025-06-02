import classNames from 'classnames/bind'
import { Link } from 'react-router-dom'
import styles from './Login.module.scss'
import CircleButton from '../CircleButton'
import { loginItems, loginIconMapper } from '~/constants/loginOptions'

const cx = classNames.bind(styles)

function Login({ onClose, isOpen }) {
  return (
    <div className={cx('container', { open: isOpen })}>
      <div className={cx('wrapper')}>
        <div className={cx('modal-wrapper')}>
          <div className={cx('login-wrapper')}>
            <div className={cx('home-wrapper')}>
              <h2 className={cx('title')}>Log in to TikTok</h2>
              <div className={cx('login-option-wrapper')}>
                {loginItems.map((item) => {
                  const Icon = loginIconMapper[item.key]

                  return (
                    <Link key={item.key} to={item.url} className={cx('login-option')}>
                      <div className={cx('iconwrapper')}>
                        <Icon />
                      </div>
                    </Link>
                  )
                })}
              </div>
            </div>
          </div>
        </div>
        <CircleButton close large onClick={onClose} className={cx('close')} />
      </div>
    </div>
  )
}

export default Login
