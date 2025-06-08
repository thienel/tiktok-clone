import { useState } from 'react'
import classNames from 'classnames/bind'
import { Link } from 'react-router-dom'
import styles from './LoginModal.module.scss'
import CircleButton from '~/components/CircleButton'
import { loginItems, loginIconMapper } from '~/constants/loginOptions'
import Login from './Login'
import Register from './Register'

const cx = classNames.bind(styles)

function LoginModal({ onClose, isOpen }) {
  const [type, setType] = useState('login')
  const [method, setMethod] = useState(null)
  const [hidePolicy, setHidePolicy] = useState(false)

  const QrIcon = loginIconMapper['qr']
  const UsernameIcon = loginIconMapper['username']

  const handleClose = () => {
    setTimeout(() => {
      setMethod(null)
      setType('login')
    }, 500)
    onClose()
  }

  return (
    <div className={cx('container', { open: isOpen })}>
      <div className={cx('wrapper')}>
        <div className={cx('modal-wrapper')}>
          <div className={cx('login-wrapper')}>
            <div className={cx('home-wrapper')}>
              {!method && <h2 className={cx('title')}>{type === 'login' ? 'Log in to' : 'Sign up for'} TikTok</h2>}
              {!method && (
                <div className={cx('login-option-wrapper')}>
                  {type === 'login' && (
                    <div className={cx('login-option')} onClick={() => setMethod('qr')}>
                      <div className={cx('iconwrapper')}>
                        <QrIcon />
                      </div>
                      <h3 className={cx('login-title')}>Use QR code</h3>
                    </div>
                  )}
                  <div className={cx('login-option')} onClick={() => setMethod('username')}>
                    <div className={cx('iconwrapper')}>
                      <UsernameIcon />
                    </div>
                    {type === 'login' ? (
                      <h3 className={cx('login-title')}>Use email or username</h3>
                    ) : (
                      <h3 className={cx('login-title')}>Continue with email</h3>
                    )}
                  </div>
                  {loginItems.map((item) => {
                    const Icon = loginIconMapper[item.key]
                    return (
                      <Link key={item.key} to={item.url} className={cx('login-option')}>
                        <div className={cx('iconwrapper')}>
                          <Icon />
                        </div>
                        <h3 className={cx('login-title')}>{item.title}</h3>
                      </Link>
                    )
                  })}
                </div>
              )}

              {method === 'username' && type === 'login' && <Login />}

              {method === 'username' && type === 'register' && <Register setHidePolicy={setHidePolicy} />}
            </div>
          </div>
          {(!(type === 'login' && method) || !hidePolicy) && (
            <div className={cx('policy-confirm')}>
              <p>
                By continuing with an account located in <a href="/">Vietnam</a>, you agree to our{' '}
                <a target="_blank" rel="noopener noreferrer" href="https://www.tiktok.com/legal/terms-of-use?lang=en">
                  Terms of Service
                </a>{' '}
                and acknowledge that you have read our{' '}
                <a target="_blank" rel="noopener noreferrer" href="https://www.tiktok.com/legal/privacy-policy?lang=en">
                  Privacy Policy
                </a>
                .
              </p>
            </div>
          )}
          <div className={cx('footer')}>
            {type === 'login' ? (
              <div>
                Don’t have an account?{' '}
                <span
                  onClick={() => {
                    setMethod(null)
                    setType('register')
                  }}
                >
                  Sign up
                </span>
              </div>
            ) : (
              <div>
                Already have an account?{' '}
                <span
                  onClick={() => {
                    setMethod(null)
                    setType('login')
                  }}
                >
                  Log in
                </span>
              </div>
            )}
          </div>
        </div>
        <CircleButton close large onClick={handleClose} className={cx('close')} />
      </div>
    </div>
  )
}

export default LoginModal
