import classNames from 'classnames/bind'
import { useState, useEffect } from 'react'
import styles from './VerificationCode.module.scss'
import images from '~/assets/images'

const cx = classNames.bind(styles)

function VerificationCode({
  verificationCode,
  setVerificationCode,
  className,
  warningIconStyle,
  warningStyle,
  warningDesStyle,
  onSetValid,
  errorCode,
  onSendVerification,
  sendButtonActive,
  loading,
}) {
  const [focused, setFocused] = useState(false)
  const [warning, setWarning] = useState(false)

  useEffect(() => {
    if (!focused) {
      const valid = isValidCode(verificationCode)
      setWarning(!valid)
    } else {
      setWarning(false)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [focused])

  useEffect(() => {
    onSetValid(verificationCode ? isValidCode(verificationCode) : false)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [verificationCode])

  const isValidCode = (code) => {
    if (!code) return true
    const regex = /^\d{6}$/
    return regex.test(code)
  }

  return (
    <>
      <div className={cx(className, warning ? warningStyle : '')}>
        <input
          value={verificationCode}
          onChange={(e) => setVerificationCode(e.target.value)}
          placeholder="Enter 6-digit code"
          onFocus={() => setFocused(true)}
          onBlur={() => setFocused(false)}
        />
        {warning && (
          <div className={warningIconStyle}>
            <images.invalid />
          </div>
        )}
        <button
          className={cx('sendcodebutton', { active: sendButtonActive, loading: loading })}
          onClick={onSendVerification}
        >
          Send code
          <div className={cx('loadingIcon')}>
            <images.loading style={{ margin: '0', width: '20', height: '20' }} />
          </div>
        </button>
      </div>
      {warning && <div className={warningDesStyle}>Enter 6-digit code</div>}
      {(errorCode === 'VERIFICATION_CODE_NOT_FOUND' ||
        errorCode === 'VERIFICATION_CODE_EXPIRED' ||
        errorCode === 'INVALID_VERIFICATION_CODE') && (
        <div className={warningDesStyle}>Verification code is expired or incorrect. Try again.</div>
      )}
    </>
  )
}

export default VerificationCode
