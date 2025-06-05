import { useState, useEffect, useRef } from 'react'
import classNames from 'classnames/bind'
import styles from './Register.module.scss'
import { useAuth } from '~/hooks'
import SelectorDropdown from './SelectorDropdown'
import { MONTHS } from '~/constants'
import PasswordInput from './PasswordInput'
import { isValidDate, getAge } from '~/utils/dateAndTime'
import EmailInput from './EmailInput'
import VerificationCode from './VerificationCodeInput'

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
  const [allFieldValid, setAllFieldValid] = useState({
    birthday: false,
    email: false,
    password: false,
    verificationCode: true,
  })
  const [canNext, setCanNext] = useState(false)

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
    birthdayValidation()
    if (!allFieldValid.birthday || !allFieldValid.email) return

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
      return
    }
    if (getAge(year, monthValue, day) < 12) {
      setValidBirthday('Sorry, looks like you’re not eligible for TikTok... But thanks for checking us out!')
      return
    }
    setValidBirthday('')
  }

  useEffect(() => {
    setValidBirthday('')
    const monthValue = MONTHS.find((m) => m.name === month)?.value
    setAllFieldValid((prev) => ({
      ...prev,
      birthday: isValidDate(year, monthValue, day) && !(getAge(year, monthValue, day) < 12),
    }))
  }, [day, month, year])

  useEffect(() => {
    setCanNext(
      allFieldValid.birthday && allFieldValid.email && allFieldValid.password && allFieldValid.verificationCode,
    )
    console.log(allFieldValid)
  }, [allFieldValid])

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
      <EmailInput
        email={email}
        setEmail={setEmail}
        className={cx('inputwrapper')}
        warningIconStyle={cx('warningIcon')}
        warningStyle={cx('warningInput')}
        warningDesStyle={cx('warningSpan')}
        onSetValid={(value) => setAllFieldValid((prev) => ({ ...prev, email: value }))}
        errorCode={error}
      />
      <PasswordInput
        password={password}
        setPassword={setPassword}
        className={cx('inputwrapper')}
        warningIconStyle={cx('warningIcon')}
        warningStyle={cx('warningInput')}
        warningDesStyle={cx('warningSpan')}
        onSetValid={(value) => setAllFieldValid((prev) => ({ ...prev, password: value }))}
      />
      <VerificationCode
        verificationCode={verificationCode}
        setVerificationCode={setVerificationCode}
        className={cx('inputwrapper')}
        warningIconStyle={cx('warningIcon')}
        warningStyle={cx('warningInput')}
        warningDesStyle={cx('warningSpan')}
        onSetValid={(value) => setAllFieldValid((prev) => ({ ...prev, verificationCode: value }))}
        onSendVerification={handleSendVerification}
        sendButtonActive={sendEmailButtonActive}
        loading={loading}
      />
      <button className={cx('submitbutton', { disabled: !canNext })}>Next</button>
    </div>
  )
}

export default Register
