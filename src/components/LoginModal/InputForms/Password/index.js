import { useEffect, useState } from 'react'
import classNames from 'classnames/bind'
import stylesPassword from './Password.module.scss'
import stylesInput from '../InputForms.module.scss'
import images from '~/assets/images'
import { checkPasswordAcceptedChar, checkPasswordLength, checkPasswordSpecialChar } from '~/utils/validation'

const cxPassword = classNames.bind(stylesPassword)
const cxInput = classNames.bind(stylesInput)

function Password({ password, setPassword, setValid }) {
  const [passwordVisible, setPasswordVisible] = useState(false)
  const [focused, setFocused] = useState(false)
  const [lengthValid, setLengthValid] = useState(false)
  const [specialCharValid, setSpecialCharValid] = useState(false)
  const [touched, setTouched] = useState(false)
  const [warningDes, setWarningDes] = useState(false)

  useEffect(() => {
    if (!password) {
      setTouched(false)
    }
    const lengthValid = checkPasswordLength(password)
    const specialCharValid = checkPasswordSpecialChar(password)
    setValid(password ? lengthValid && specialCharValid : false)
    setWarningDes(!checkPasswordAcceptedChar(password))
    setLengthValid(lengthValid)
    setSpecialCharValid(specialCharValid)
    // eslint-disable-next-line react-hooks/exhaustive-deps
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
      <div className={cxInput('wrapper', showValidation() ? 'warningInput' : '')}>
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
            <images.invalid className={cxInput('warningIcon')} />
          </div>
        )}
        <div className={cxPassword('passwordIconWrapper')} onClick={() => setPasswordVisible((prev) => !prev)}>
          {passwordVisible ? <images.passwordView /> : <images.passwordHide />}
        </div>
      </div>

      {(focused || showValidation()) && (
        <>
          <div className={cxInput('warningDes')} style={{ display: warningDes ? 'block' : 'none' }}>
            Invalid special character
          </div>
          <p className={cxPassword('pwValidationTitle')}>Your password must have:</p>

          <div className={cxPassword('pwValidationDes', getStatusClass(lengthValid))}>
            <images.valid style={{ marginRight: '4px' }} fill="currentColor" />
            <span>8 to 20 characters</span>
          </div>

          <div
            className={cxPassword('pwValidationDes', getStatusClass(specialCharValid))}
            style={{ marginBottom: '9px' }}
          >
            <images.valid style={{ marginRight: '4px' }} fill="currentColor" />
            <span>Letters, numbers, and special characters</span>
          </div>
        </>
      )}
    </>
  )
}

export default Password
