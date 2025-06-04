import { useState, useEffect, useRef } from 'react'
import classNames from 'classnames/bind'
import styles from './Register.module.scss'
import images from '~/assets/images'
import { useAuth } from '~/hooks'
import SelectorDropdown from './SelectorDropdown'

const cx = classNames.bind(styles)

function Register({ open }) {
  const [dropdownField, setDropdownField] = useState(null)
  const monthRef = useRef()
  const dayRef = useRef()
  const yearRef = useRef()
  const [month, setMonth] = useState('')
  const [day, setDay] = useState('')
  const [year, setYear] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [verificationCode, setVerificationCode] = useState('')
  const { sendEmailVerification, loading, error } = useAuth()

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

  const handleSelectMonth = (monthName) => {
    setMonth(monthName)
    setDropdownField(null)
  }

  const handleSelectDay = (dayValue) => {
    setDay(dayValue)
    setDropdownField(null)
  }

  const handleSelectYear = (yearValue) => {
    setYear(yearValue)
    setDropdownField(null)
  }

  const handleSendVerification = async () => {
    if (!sendEmailButtonActive) return

    try {
      const result = await sendEmailVerification(email)
      console.log(result)
    } catch (err) {
      console.error('Error sending verification:', err)
    }
  }

  return (
    <div className={cx('wrapper', { open })}>
      <h2 className={cx('title')}>Sign up</h2>
      <div className={cx('title-birthday')}>When's your birthday?</div>
      <div className={cx('age-selector')}>
        <div
          className={cx('selector', { show: dropdownField === 'month', selected: !!month })}
          onClick={() => {
            dropdownField === month ? setDropdownField(null) : setDropdownField('month')
          }}
        >
          {month || 'Month'}
          <div className={cx('iconwrapper')}>
            <images.selector />
          </div>
          {dropdownField === 'month' && <SelectorDropdown type={'month'} ref={monthRef} onSelect={handleSelectMonth} />}
        </div>

        <div
          className={cx('selector', { show: dropdownField === 'day', selected: !!day })}
          onClick={() => {
            dropdownField === 'day' ? setDropdownField(null) : setDropdownField('day')
          }}
        >
          {day || 'Day'}
          <div className={cx('iconwrapper')}>
            <images.selector />
          </div>
          {dropdownField === 'day' && (
            <SelectorDropdown type={'day'} ref={dayRef} onSelect={handleSelectDay} day={day} year={year} />
          )}
        </div>

        <div
          className={cx('selector', { show: dropdownField === 'year', selected: !!year })}
          onClick={() => {
            dropdownField === 'year' ? setDropdownField(null) : setDropdownField('year')
          }}
        >
          {year || 'Year'}
          <div className={cx('iconwrapper')}>
            <images.selector />
          </div>
          {dropdownField === 'year' && <SelectorDropdown type={'year'} ref={yearRef} onSelect={handleSelectYear} />}
        </div>
      </div>
      <span className={cx('age-validation')}>Your birthday won't be shown publicly.</span>

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
          className={cx('sendcodebutton', { active: sendEmailButtonActive })}
          onClick={handleSendVerification}
          disabled={!sendEmailButtonActive || loading}
        >
          {loading ? 'Sending...' : 'Send code'}
        </button>
      </div>

      {error && <div className={cx('error-message')}>{error}</div>}
    </div>
  )
}

export default Register
