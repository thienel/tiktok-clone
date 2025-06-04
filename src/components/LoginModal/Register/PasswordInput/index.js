import { useEffect, useRef, useState } from 'react'
import classNames from 'classnames/bind'
import styles from './PasswordInput.module.scss'
import images from '~/assets/images'

const cx = classNames.bind(styles)

function PasswordInput({ password, setPassword, className }) {
  const [passwordVisible, setPasswordVisible] = useState(false)
  const [passwordFocused, setPasswordFocused] = useState(false)

  const passwordRef = useRef()

  useEffect(() => {
    const handleFocus = () => setPasswordFocused(true)
    const handleBlur = () => setPasswordFocused(false)

    passwordRef.current.addEventListener('focus', handleFocus)
    passwordRef.current.addEventListener('blur', handleBlur)

    return () => {
      passwordRef.current.removeEventListener('focus', handleFocus)
      passwordRef.current.removeEventListener('blur', handleBlur)
    }
  }, [])

  return (
    <>
      <div className={className} ref={passwordRef}>
        <input
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="Password"
          type={passwordVisible ? 'text' : 'password'}
        />
        <div
          className={cx('passwordIconWrapper')}
          onClick={() => {
            setPasswordVisible((prev) => !prev)
            setPasswordFocused(true)
          }}
        >
          {passwordVisible ? <images.passwordView /> : <images.passwordHide />}
        </div>
      </div>
      {passwordFocused && (
        <>
          <p class={cx('pwValidationTitle')}>Your password must have:</p>
          <div className={cx('pwValidationDes')}>
            <images.valid style={{ marginRight: '4px' }} />
            <span>8 to 20 characters</span>
          </div>
          <div className={cx('pwValidationDes')} style={{ marginBottom: '9px' }}>
            <images.valid style={{ marginRight: '4px' }} />
            <span>Letters, numbers, and special characters</span>
          </div>
        </>
      )}
    </>
  )
}

export default PasswordInput
