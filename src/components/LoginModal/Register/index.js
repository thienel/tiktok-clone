import { useState, useEffect, useRef } from 'react'
import classNames from 'classnames/bind'
import styles from './Register.module.scss'
import { useAuth } from '~/hooks'
import SelectorDropdown from './SelectorDropdown'
import images from '~/assets/images'
import { MONTHS } from '~/constants'

const cx = classNames.bind(styles)

function Register({ open }) {
  const [dropdownField, setDropdownField] = useState(null)
  const [month, setMonth] = useState('')
  const [day, setDay] = useState('')
  const [year, setYear] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [verificationCode, setVerificationCode] = useState('')
  const { sendEmailVerification, loading, error } = useAuth()
  const [validBirthday, setValidBirthday] = useState('')

  const monthRef = useRef()
  const dayRef = useRef()
  const yearRef = useRef()

  const sendEmailButtonActive = !!month && !!day && !!year && !!email

  const handleClickOutside = (e) => {
    if (dropdownField) {
      const ref =
        dropdownField === 'month'
          ? monthRef
          : dropdownField === 'day'
          ? dayRef
          : dropdownField === 'year'
          ? yearRef
          : null
      if (ref && ref?.current && !ref.current.contains(e.target)) {
        setDropdownField(null)
      }
    }
  }

  useEffect(() => {
    if (dropdownField) {
      window.addEventListener('mousedown', handleClickOutside)
    }

    return () => {
      window.removeEventListener('mousedown', handleClickOutside)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dropdownField])

  const handleSendVerification = async () => {
    if (!birthdayValidation()) return
    if (!sendEmailButtonActive) return

    try {
      const result = await sendEmailVerification(email)
      console.log(result)
    } catch (err) {
      console.error('Error sending verification:', err)
    }
  }

  const birthdayValidation = () => {
    const monthValue = MONTHS.find((m) => m.name === month)?.value
    if (!isValidDate(year, monthValue, day)) {
      setValidBirthday('Enter a valid date')
      return false
    }
    if (getAge(year, monthValue, day) < 12) {
      setValidBirthday('Sorry, looks like you’re not eligible for TikTok... But thanks for checking us out!')
      return false
    }
    setValidBirthday('')
    return true
  }

  function isValidDate(year, month, day) {
    const date = new Date(year, month - 1, day)
    return (
      date.getFullYear() === parseInt(year) &&
      date.getMonth() === parseInt(month) - 1 &&
      date.getDate() === parseInt(day)
    )
  }

  function getAge(year, month, day) {
    const today = new Date()
    const birthDate = new Date(year, month - 1, day)

    let age = today.getFullYear() - birthDate.getFullYear()
    const m = today.getMonth() - birthDate.getMonth()

    if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
      age--
    }

    return age
  }

  useEffect(() => {
    setValidBirthday('')
  }, [day, month, year])

  return (
    <div className={cx('wrapper', { open })}>
      <h2 className={cx('title')}>Sign up</h2>
      <div className={cx('title-birthday')}>When's your birthday?</div>
      <div className={cx('age-selector')}>
        <SelectorDropdown
          type={'month'}
          ref={monthRef}
          setValue={setMonth}
          month={month}
          dropdownField={dropdownField}
          setDropdownField={setDropdownField}
          invalid={!!validBirthday}
        />
        <SelectorDropdown
          type={'day'}
          ref={dayRef}
          setValue={setDay}
          month={month}
          day={day}
          year={year}
          dropdownField={dropdownField}
          setDropdownField={setDropdownField}
          invalid={!!validBirthday}
        />
        <SelectorDropdown
          type={'year'}
          ref={yearRef}
          setValue={setYear}
          year={year}
          dropdownField={dropdownField}
          setDropdownField={setDropdownField}
          invalid={!!validBirthday}
        />
      </div>
      <span className={cx('age-validation', { notvalid: !!validBirthday })}>
        {validBirthday ? validBirthday : "Your birthday won't be shown publicly."}
      </span>

      <div className={cx('title-method')}>Email</div>
      <div className={cx('inputwrapper')}>
        <input value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email address" type="email" />
      </div>
      <div className={cx('inputwrapper')}>
        <input
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="Password"
          type="password"
          minLength="6"
        />
      </div>
      <div className={cx('inputwrapper')}>
        <input
          value={verificationCode}
          onChange={(e) => setVerificationCode(e.target.value)}
          placeholder="Enter 6-digit code"
          maxLength="6"
        />
        <button
          className={cx('sendcodebutton', { active: sendEmailButtonActive, loading: loading })}
          onClick={handleSendVerification}
        >
          Send code
          <div className={cx('loadingIcon')}>
            <images.loading style={{ margin: '0', width: '20', height: '20' }} />
          </div>
        </button>
      </div>

      {error && <div className={cx('error-message')}>{error}</div>}
    </div>
  )
}

export default Register
