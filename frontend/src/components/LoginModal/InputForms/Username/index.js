import { useEffect, useState } from 'react'
import classNames from 'classnames/bind'
import stylesInput from '~/components/LoginModal/InputForms/InputForms.module.scss'
import images from '~/assets/images'
import { useUsersAPI } from '~/hooks'

const cxInput = classNames.bind(stylesInput)

function Username({ username, setUsername, setValid }) {
  const [warningMessage, setWarningMessage] = useState('')
  const { checkUsername, isCheckingUsername } = useUsersAPI()

  useEffect(() => {
    if (!username) {
      setWarningMessage('')
      setValid(false)
      return
    }

    const warning = validateUsername(username)
    if (warning) {
      setWarningMessage(warning)
      setValid(false)
    } else {
      setWarningMessage('')
      setValid(false)
    }

    const check = async () => {
      try {
        const result = await checkUsername(username)
        if (result.errorCode === 'USERNAME_USED') {
          setWarningMessage("This username isn't available. Try a new one.")
          setValid(false)
        } else {
          setWarningMessage('')
          setValid(true)
        }
      } catch (err) {
        console.error('API Error:', err)
        setValid(false)
      }
    }

    if (!warning) {
      check()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [username])

  const validateUsername = (username) => {
    if (!/^[a-zA-Z0-9_.]*$/.test(username)) {
      return 'Only letters, numbers, underscores, or periods are allowed'
    }
    if (username.length < 2) {
      return 'Include at least 2 characters in your username'
    }
    if (username.length > 24) {
      return 'Your username can have up to 24 characters'
    }
    return ''
  }

  return (
    <>
      <div className={cxInput('wrapper', warningMessage ? 'warningInput' : '')}>
        <input
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          placeholder="Username"
          spellCheck="false"
        />
        {!!warningMessage && (
          <div className={cxInput('warningIcon')}>
            <images.invalid />
          </div>
        )}
        {!warningMessage && isCheckingUsername && (
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
