import { useEffect, useState } from 'react'
import classNames from 'classnames/bind'
import stylesInput from '../InputForms.module.scss'
import images from '~/assets/images'
import { useAuth, useDebounce } from '~/hooks'

const cxInput = classNames.bind(stylesInput)

function Username({ username, setUsername, setValid }) {
  const [warningMessage, setWarningMessage] = useState('')
  const [error, setError] = useState('')
  const { checkUsername, loading, LOADING_TYPE } = useAuth()
  const debouncedUsername = useDebounce(username, 500)

  useEffect(() => {
    if (warningMessage || !username) return

    const check = async () => {
      try {
        const result = await checkUsername(debouncedUsername)
        console.log(result)
        if (result.errorCode) {
          setError(result.errorCode)
          setValid(false)
        } else {
          setError('')
        }
      } catch (err) {
        console.error(err)
      }
    }

    check()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [debouncedUsername])

  useEffect(() => {
    if (error === 'USERNAME_USED') setWarningMessage("This username isn't available. Try enter a new one.")
  }, [error])

  useEffect(() => {
    const charValid = checkAcceptedChar(username)
    const minLengthValid = checkMinLength(username)
    const maxLengthValid = checkMaxLength(username)
    setValid(charValid && minLengthValid && maxLengthValid)
    if (!username || (charValid && minLengthValid && maxLengthValid)) {
      setWarningMessage('')
    } else if (!charValid) {
      setWarningMessage('Only letters, numbers, underscores, or periods are allowed')
    } else if (!minLengthValid) {
      setWarningMessage('Include at least 2 characters in your username')
    } else if (!maxLengthValid) {
      setWarningMessage('Your username can have up to 24 characters')
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [username])

  const checkMinLength = (username) => username.length >= 2
  const checkMaxLength = (username) => username.length <= 24
  const checkAcceptedChar = (username) => {
    const regex = /^[a-zA-Z0-9_.]*$/
    return regex.test(username)
  }

  return (
    <>
      <div className={cxInput('wrapper', warningMessage ? 'warningInput' : '')}>
        <input value={username} onChange={(e) => setUsername(e.target.value)} placeholder="Username" />
        {!!warningMessage && (
          <div className={cxInput('warningIcon')}>
            <images.invalid />
          </div>
        )}
        {!warningMessage && loading === LOADING_TYPE.CHECK_USERNAME && (
          <div className="loadingIcon">
            <images.loading />
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
