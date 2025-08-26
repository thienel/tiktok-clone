import classNames from 'classnames/bind'
import { useEffect, useState } from 'react'
import stylesEmail from './Email.module.scss'
import images from '~/assets/images'
import stylesInput from '~/components/LoginModal/InputForms/InputForms.module.scss'
import { isValidEmailFormat } from '~/utils/validation'

const cxEmail = classNames.bind(stylesEmail)
const cxInput = classNames.bind(stylesInput)

function Email({ email, setEmail, setValid, errorCode }) {
  const [focused, setFocused] = useState(false)
  const [warning, setWarning] = useState(false)

  useEffect(() => {
    if (!focused) {
      const valid = isValidEmailFormat(email)
      setWarning(!valid)
      setValid(email ? valid : false)
    } else {
      setWarning(false)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [focused])

  return (
    <>
      <div className={cxInput('wrapper', warning ? 'warningInput' : '')}>
        <input
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder="Email address"
          type="email"
          onFocus={() => setFocused(true)}
          onBlur={() => setFocused(false)}
        />
        {warning && (
          <div className={cxInput('warningIcon')}>
            <images.invalid />
          </div>
        )}
      </div>
      {warning && <div className={cxInput('warningDes')}>Enter a valid email address</div>}
      {(errorCode === 'EMAIL_ALREADY_CONFIRMED' || errorCode === 'EMAIL_USED') && (
        <p className={cxEmail('confirmedSpan')}>
          You’ve already signed up,
          <span className={cxEmail('loginLink')}>
            Log in
            <images.flipLTR color="currentColor" height="12" width="12" />
          </span>
        </p>
      )}
    </>
  )
}

export default Email
