import classNames from 'classnames/bind'
import styles from './SelectorDropdown.module.scss'
import { useMemo } from 'react'
import images from '~/assets/images'
import { MONTHS } from '~/constants'

const cx = classNames.bind(styles)

function SelectorDropdown({ ref, setValue, type, month, day, year, dropdownField, setDropdownField, invalid }) {
  const handleSetValue = (value) => {
    setValue(value)
    setDropdownField(null)
  }

  const handleToggleDropdown = (e) => {
    e.stopPropagation()
    dropdownField === type ? setDropdownField(null) : setDropdownField(type)
  }

  const handleItemClick = (e, itemName) => {
    e.stopPropagation()
    handleSetValue(itemName)
  }

  const DAYS = useMemo(() => {
    if (!month || !year) {
      return Array.from({ length: 31 }, (_, i) => ({
        name: String(i + 1).padStart(2, '0'),
        value: String(i + 1).padStart(2, '0'),
      }))
    }
    const monthValue = MONTHS.find((m) => m.name === month)?.value
    if (!monthValue) return []
    const daysInMonth = new Date(parseInt(year), parseInt(monthValue), 0).getDate()
    return Array.from({ length: daysInMonth }, (_, i) => ({
      name: String(i + 1).padStart(2, '0'),
      value: String(i + 1).padStart(2, '0'),
    }))
  }, [year, month])

  const YEARS = useMemo(() => {
    const currentYear = new Date().getFullYear()
    const years = []
    for (let i = currentYear; i >= 1900; i--) {
      years.push({
        name: String(i),
        value: String(i),
      })
    }
    return years
  }, [])

  const list = type === 'day' ? DAYS : type === 'year' ? YEARS : MONTHS

  const selectedType = (type === 'day' && !!day) || (type === 'month' && !!month) || (type === 'year' && !!year)

  return (
    <div
      className={cx('selector', {
        show: dropdownField === type,
        selected: selectedType,
        invalid,
      })}
      ref={ref}
      onClick={handleToggleDropdown}
    >
      {type === 'month' && (month || 'Month')}
      {type === 'day' && (day || 'Day')}
      {type === 'year' && (year || 'Year')}

      <div className={cx('iconwrapper')}>
        <images.selector />
      </div>

      {dropdownField === type && (
        <div className={cx('listbox')}>
          {list.map((item) => (
            <div key={item.value} className={cx('listitem')} onClick={(e) => handleItemClick(e, item.name)}>
              {item.name}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

export default SelectorDropdown
