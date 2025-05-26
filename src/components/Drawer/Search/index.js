import classNames from 'classnames/bind'
import styles from './Search.module.scss'
import CircleButton from '~/components/CircleButton'
import images from '~/assets/images'
import { useEffect, useRef, useState } from 'react'

const cx = classNames.bind(styles)

function Search({ onExpand }) {
  const inputRef = useRef()

  const [isFocused, setIsFocused] = useState(false)
  const [inputValue, setInputValue] = useState('')

  useEffect(() => {
    console.log(inputValue)
  }, [inputValue])

  const handleClear = () => {
    inputRef.current.focus()
    setInputValue('')
  }

  return (
    <div className={cx('wrapper')}>
      <div className={cx('searchWrapper')}>
        <h2 className={cx('header')}>Search</h2>
        <div className={cx('inputWrapper', { focused: isFocused })}>
          <input
            className={cx('input')}
            placeholder="Search"
            onFocus={() => setIsFocused(true)}
            onBlur={() => setIsFocused(false)}
            ref={inputRef}
            value={inputValue}
            onChange={(e) => setInputValue(e.target.value)}
          />
          {inputValue && (
            <div className={cx('clear')} onClick={handleClear}>
              <images.clear />
            </div>
          )}
          <div className={cx('loading')}>
            <images.loading />
          </div>
        </div>
      </div>
      <div className={cx('content')}></div>
      <div className={cx('close')}>
        <CircleButton close onClick={onExpand} />
      </div>
    </div>
  )
}

export default Search
