import { useState, useEffect, useRef } from 'react'
import classNames from 'classnames/bind'
import stylesBirthday from './BirthdaySelector.module.scss'
import stylesInput from '~/components/LoginModal/InputForms/InputForms.module.scss'
import SelectorDropdown from './SelectorDropdown'
import { isValidDate } from '~/utils/validation'
import { MONTHS } from '~/constants'

const cxBirthday = classNames.bind(stylesBirthday)
const cxInput = classNames.bind(stylesInput)

function BirthdaySelector({ setBirthDate, errorCode, setValid }) {
  const [month, setMonth] = useState('')
  const [day, setDay] = useState('')
  const [year, setYear] = useState('')
  const [warning, setWarning] = useState(false)

  const [dropdownField, setDropdownField] = useState(null)
  const monthRef = useRef()
  const dayRef = useRef()
  const yearRef = useRef()

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

  useEffect(() => {
    const monthValue = MONTHS.find((m) => m.name === month)?.value
    const isValid = isValidDate(year, monthValue, day)
    setValid(isValid)
    if (!day || !month || !year) {
      setBirthDate('')
      setWarning(false)
    } else {
      setWarning(!isValid)
      const birthDate = year + '-' + monthValue + '-' + day
      setBirthDate(birthDate)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [day, month, year])

  return (
    <>
      <div className={cxBirthday('title')}>When's your birthday?</div>
      <div className={cxBirthday('selector')}>
        <SelectorDropdown
          type={'month'}
          ref={monthRef}
          setValue={setMonth}
          month={month}
          dropdownField={dropdownField}
          setDropdownField={setDropdownField}
          invalid={warning}
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
          invalid={warning}
        />
        <SelectorDropdown
          type={'year'}
          ref={yearRef}
          setValue={setYear}
          year={year}
          dropdownField={dropdownField}
          setDropdownField={setDropdownField}
          invalid={warning}
        />
      </div>
      <span className={cxInput('description', { warning: warning || errorCode === 'INVALID_BIRTH_DATE' })}>
        {errorCode === 'INVALID_BIRTH_DATE'
          ? 'Sorry, looks like you’re not eligible for TikTok... But thanks for checking us out!'
          : warning
          ? 'Enter a valid date'
          : "Your birthday won't be shown publicly."}
      </span>
    </>
  )
}

export default BirthdaySelector
