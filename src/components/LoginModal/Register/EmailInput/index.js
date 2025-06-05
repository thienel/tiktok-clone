import classNames from 'classnames/bind'
import { useEffect, useState } from 'react'
import styles from './EmailInput.module.scss'
import images from '~/assets/images'

const cx = classNames.bind(styles)

function EmailInput({
  email,
  setEmail,
  className,
  warningIconStyle,
  warningStyle,
  warningDesStyle,
  onSetValid,
  errorCode,
}) {
  const [focused, setFocused] = useState(false)
  const [warning, setWarning] = useState(false)

  useEffect(() => {
    if (!focused) {
      const valid = isValidEmail(email)
      setWarning(!valid)
      onSetValid(email ? valid : false)
    } else {
      setWarning(false)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [focused])

  function isValidEmail(email) {
    if (!email) return true
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    return regex.test(email)
  }

  return (
    <>
      <div className={`${className} ${warning ? warningStyle : ''}`}>
        <input
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder="Email address"
          type="email"
          onFocus={() => setFocused(true)}
          onBlur={() => setFocused(false)}
        />
        {warning && (
          <div className={warningIconStyle}>
            <images.invalid />
          </div>
        )}
      </div>
      {warning && <div className={warningDesStyle}>Enter a valid email address</div>}
      {(errorCode === 'EMAIL_ALREADY_CONFIRMED' || errorCode === 'EMAIL_USED') && (
        <p class={cx('confirmedSpan')}>
          You’ve already signed up,
          <span className={cx('loginLink')}>
            Log in
            <images.flipLTR color="currentColor" height="12" width="12" />
          </span>
        </p>
      )}
    </>
  )
}

export default EmailInput
