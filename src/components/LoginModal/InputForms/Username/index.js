import { useEffect, useState } from 'react'
import classNames from 'classnames/bind'
import stylesInput from '../InputForms.module.scss'
import images from '~/assets/images'

const cxInput = classNames.bind(stylesInput)

function Username({ username, setUsername, open, setValid }) {
  const [warningMessage, setWarningMessage] = useState('')

  useEffect(() => {
    if (!username) setWarningMessage('')
    const charValid = checkAcceptedChar(username)
    const minLengthValid = checkMinLength(username)
    const maxLengthValid = checkMaxLength(username)
    setValid(charValid && minLengthValid && maxLengthValid)
    if (!charValid) {
      setWarningMessage('Only letters, numbers, underscores, or periods are allowed')
    } else if (!minLengthValid) {
      setWarningMessage('Include at least 2 characters in your username')
    } else if (!maxLengthValid) {
      setWarningMessage('Your username can have up to 24 characters')
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [username])

  const checkMinLength = (username) => username.length < 2
  const checkMaxLength = (username) => username.length > 24
  const checkAcceptedChar = (username) => {
    const regex = /^[a-zA-Z0-9_.]*$/
    return regex.test(username)
  }

  return (
    <>
      <div className={cxInput('wrapper', { open })}>
        <input value={username} onChange={(e) => setUsername(e.target.value)} placeholder="Username" />
        {!!warningMessage && (
          <div className={cxInput('warningIcon')}>
            <images.invalid />
          </div>
        )}
      </div>
      <span className={cxInput('description', { warning: !!warningMessage })}>
        {!warningMessage ? 'You can always change this later.' : warningMessage}
      </span>
    </>
  )
}

export default Username
