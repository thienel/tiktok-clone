import { useEffect, useState } from 'react'
import images from '~/assets/images'

function EmailInput({ email, setEmail, className, warningIconStyle, warningStyle, warningDesStyle }) {
  const [focused, setFocused] = useState(false)
  const [warning, setWarning] = useState(false)

  useEffect(() => {
    if (!focused) {
      setWarning(!isValidEmail(email))
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
    </>
  )
}

export default EmailInput
