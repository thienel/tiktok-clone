import { useEffect, useState } from 'react'
import classNames from 'classnames/bind'
import stylesInput from '~/components/LoginModal/InputForms/InputForms.module.scss'
import stylesPassword from '~/components/LoginModal/InputForms/Password/Password.module.scss'
import images from '~/assets/images'

const cxPassword = classNames.bind(stylesPassword)
const cxInput = classNames.bind(stylesInput)

function Default({ value, setValue, placeholder, type = 'text', errorCode, clearErrorCode }) {
  const [passwordVisible, setPasswordVisible] = useState(false)

  const rederType = () => {
    if (type !== 'password') return type

    return passwordVisible ? 'text' : 'password'
  }

  const errorMessage = () => {
    if (!errorCode) return ''

    if (errorCode === 'INVALID_CREDENTIALS') return "Username or password doesn't match our records. Try again."

    if (errorCode === 'ACCOUNT_LOCKED') return 'Incorrect account or password trials limits. Try again in 15 minute.'

    return ''
  }

  useEffect(() => {
    if (errorCode) {
      clearErrorCode()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value])

  return (
    <>
      <div className={cxInput('wrapper', { warningInput: errorCode })}>
        <input
          value={value}
          onChange={(e) => setValue(e.target.value)}
          placeholder={placeholder}
          spellCheck="false"
          type={rederType()}
        />
        {errorCode && (
          <div>
            <images.invalid className={cxInput('warningIcon')} />
          </div>
        )}
        {type === 'password' && (
          <div className={cxPassword('passwordIconWrapper')} onClick={() => setPasswordVisible((prev) => !prev)}>
            {passwordVisible ? <images.passwordView /> : <images.passwordHide />}
          </div>
        )}
      </div>
      {errorCode && <div className={cxInput('warningDes')}>{errorMessage()}</div>}
    </>
  )
}

export default Default
