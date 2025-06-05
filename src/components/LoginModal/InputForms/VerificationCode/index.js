import classNames from 'classnames/bind'
import { useState, useEffect } from 'react'
import stylesVerificationCode from './VerificationCode.module.scss'
import stylesInput from '../InputForms.module.scss'
import images from '~/assets/images'
import { isValidCode } from '~/utils/validation'

const cxVerificationCode = classNames.bind(stylesVerificationCode)
const cxInput = classNames.bind(stylesInput)

function VerificationCode({
  verificationCode,
  setVerificationCode,
  setValid,
  errorCode,
  onSendVerification,
  sendButtonActive,
  loading,
  onResetErrorCode,
  countdown,
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
    setValid(verificationCode ? isValidCode(verificationCode) : false)
    onResetErrorCode('')
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [verificationCode])

  useEffect(() => {
    let timer
    if (errorCode === 'WAIT_BEFORE_RESEND') {
      timer = setTimeout(() => {
        onResetErrorCode()
      }, 10000)
    }

    return () => clearTimeout(timer)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [errorCode])

  return (
    <>
      <div className={classNames(cxInput('wrapper', warning ? 'warningInput' : ''), cxVerificationCode('wrapper'))}>
        <input
          value={verificationCode}
          onChange={(e) => setVerificationCode(e.target.value)}
          placeholder="Enter 6-digit code"
          onFocus={() => setFocused(true)}
          onBlur={() => setFocused(false)}
        />
        {warning && (
          <div className={cxInput('warningIcon')}>
            <images.invalid />
          </div>
        )}
        <button
          className={cxVerificationCode('sendcodebutton', {
            active: sendButtonActive && countdown === 0,
            loading: loading,
          })}
          onClick={onSendVerification}
        >
          {countdown !== 0 ? `Resend code: ${countdown}s` : 'Send code'}
          <div className={cxVerificationCode('loadingIcon')}>
            <images.loading style={{ margin: '0', width: '20', height: '20' }} fill="currentColor" />
          </div>
        </button>
      </div>
      {warning && <div className={cxInput('warningDes')}>Enter 6-digit code</div>}
      {(errorCode === 'VERIFICATION_CODE_NOT_FOUND' ||
        errorCode === 'VERIFICATION_CODE_EXPIRED' ||
        errorCode === 'INVALID_VERIFICATION_CODE') && (
        <div className={cxInput('warningDes')}>Verification code is expired or incorrect. Try again.</div>
      )}
      {errorCode === 'WAIT_BEFORE_RESEND' && (
        <div className={cxInput('warningDes')}>Please wait 60s before resend email verification.</div>
      )}
    </>
  )
}

export default VerificationCode
