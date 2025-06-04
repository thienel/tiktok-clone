import classNames from 'classnames/bind'
import styles from './SelectorDropdown.module.scss'
import { useMemo } from 'react'

const cx = classNames.bind(styles)

const MONTHS = [
  { name: 'January', value: '01' },
  { name: 'February', value: '02' },
  { name: 'March', value: '03' },
  { name: 'April', value: '04' },
  { name: 'May', value: '05' },
  { name: 'June', value: '06' },
  { name: 'July', value: '07' },
  { name: 'August', value: '08' },
  { name: 'September', value: '09' },
  { name: 'October', value: '10' },
  { name: 'November', value: '11' },
  { name: 'December', value: '12' },
]

function SelectorDropdown({ ref, onSelect, type, month, year }) {
  const DAYS = useMemo(() => {
    if (!month && !year) {
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
  return (
    <div ref={ref} className={cx('listbox')}>
      {list.map((item) => (
        <div
          key={item.value}
          className={cx('listitem')}
          onClick={(e) => {
            e.stopPropagation()
            onSelect(item.name)
          }}
        >
          {item.name}
        </div>
      ))}
    </div>
  )
}

export default SelectorDropdown
