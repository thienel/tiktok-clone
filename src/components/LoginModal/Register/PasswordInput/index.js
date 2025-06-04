import { useEffect, useState } from 'react'
import classNames from 'classnames/bind'
import styles from './PasswordInput.module.scss'
import images from '~/assets/images'

const cx = classNames.bind(styles)

function PasswordInput({ password, setPassword, className, warningIconStyle, warningStyle, warningDesStyle }) {
  const [passwordVisible, setPasswordVisible] = useState(false)
  const [focused, setFocused] = useState(false)
  const [lengthValid, setLengthValid] = useState(false)
  const [specialCharValid, setSpecialCharValid] = useState(false)
  const [touched, setTouched] = useState(false)
  const [warningDes, setWarningDes] = useState(false)

  const checkLength = (value) => value.length >= 8 && value.length <= 20

  const acceptedChar = (value) => {
    const regex = /^[a-zA-Z0-9!@#$%^&*()_\-+=\[\]{}|;:'",.<>?\\/]*$/
    return regex.test(value)
  }

  const checkSpecialChar = (value) => {
    const regex = /^(?=.*\d)(?=.*[!@#$%^&*(),.?":{}|<>_\-+=\\/[\]`~]).+$/
    return regex.test(value)
  }

  useEffect(() => {
    if (!password) {
      setTouched(false)
    }
    setWarningDes(!acceptedChar(password))
    setLengthValid(checkLength(password))
    setSpecialCharValid(checkSpecialChar(password))
  }, [password])

  const getStatusClass = (isValid) => {
    if (isValid) return 'success'
    if (!isValid && touched) return 'warning'
  }

  const showValidation = () => {
    if (lengthValid && specialCharValid) return false
    return touched
  }

  return (
    <>
      <div className={cx(className, showValidation() ? warningStyle : '')}>
        <input
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="Password"
          type={passwordVisible ? 'text' : 'password'}
          onFocus={() => {
            if (!password) setTouched(false)
            setFocused(true)
          }}
          onBlur={() => {
            if (password) setTouched(true)
            setFocused(false)
          }}
        />
        {showValidation() && (
          <div>
            <images.invalid className={warningIconStyle} />
          </div>
        )}
        <div className={cx('passwordIconWrapper')} onClick={() => setPasswordVisible((prev) => !prev)}>
          {passwordVisible ? <images.passwordView /> : <images.passwordHide />}
        </div>
      </div>

      {(focused || showValidation()) && (
        <>
          <div className={warningDesStyle} style={{ display: warningDes ? 'block' : 'none' }}>
            Invalid special character
          </div>
          <p className={cx('pwValidationTitle')}>Your password must have:</p>

          <div className={cx('pwValidationDes', getStatusClass(lengthValid))}>
            <images.valid style={{ marginRight: '4px' }} fill="currentColor" />
            <span>8 to 20 characters</span>
          </div>

          <div className={cx('pwValidationDes', getStatusClass(specialCharValid))} style={{ marginBottom: '9px' }}>
            <images.valid style={{ marginRight: '4px' }} fill="currentColor" />
            <span>Letters, numbers, and special characters</span>
          </div>
        </>
      )}
    </>
  )
}

export default PasswordInput
