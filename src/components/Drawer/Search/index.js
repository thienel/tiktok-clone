import classNames from 'classnames/bind'
import styles from './Search.module.scss'
import CircleButton from '~/components/CircleButton'
import images from '~/assets/images'
import { useEffect, useRef, useState } from 'react'
import { useDebounce } from '~/hooks'
import * as searchServices from '~/apiServices/searchServices'

const cx = classNames.bind(styles)

function Search({ onExpand, searchValue, setSearchValue }) {
  const inputRef = useRef()
  const clearRef = useRef()
  const loadingRef = useRef()
  const [isFocused, setIsFocused] = useState(false)
  const debouncedSearchValue = useDebounce(searchValue, 500)
  const [loading, setLoading] = useState(false)
  const [searchResult, setSearchResult] = useState([])

  console.log('search render')

  useEffect(() => {
    if (!debouncedSearchValue.trim()) {
      setSearchResult([])
      setLoading(false)
      return
    }

    setLoading(true)

    const fetch = async () => {
      const result = await searchServices.search(debouncedSearchValue)
      setSearchResult(result)
      setLoading(false)
    }

    fetch()
  }, [debouncedSearchValue])

  const handleClear = () => {
    inputRef.current.focus()
    setSearchValue('')
    setSearchResult([])
  }

  useEffect(() => {
    inputRef.current.focus()
  })

  useEffect(() => {
    if (clearRef.current) {
      clearRef.current.style.display = !loading ? 'flex' : 'none'
    }
    if (loadingRef.current) {
      loadingRef.current.style.display = loading ? 'flex' : 'none'
    }
  }, [loading])

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
            value={searchValue}
            onChange={(e) => setSearchValue(e.target.value)}
            spellCheck={false}
          />
          {searchValue && (
            <div ref={clearRef} className={cx('clear')} onClick={handleClear}>
              <images.clear />
            </div>
          )}
          <div ref={loadingRef} className={cx('loading')}>
            <images.loading />
          </div>
        </div>
      </div>
      <div className={cx('content')}>
        {searchResult.length === 0 ? (
          <div className={cx('mayLike')}>
            <ul></ul>
          </div>
        ) : (
          <div className={cx('searchResult')}>
            <ul>
              <li className={cx('resultTitle')}>Accounts</li>
              {searchResult.map((item, index) => {
                return (
                  <li key={index} className={cx('resultItem')}>
                    <span className={cx('userAvt')}>
                      <img src={item.avatar} alt="" />
                    </span>
                    <div className={cx('userInfo')}>
                      <h4>
                        {item.full_name}{' '}
                        {item.tick && (
                          <span className={cx('verifyWrapper')}>
                            <images.verify />
                          </span>
                        )}
                      </h4>
                      <p>{item.nickname}</p>
                    </div>
                  </li>
                )
              })}
            </ul>
          </div>
        )}
      </div>
      <div className={cx('close')}>
        <CircleButton close onClick={onExpand} />
      </div>
    </div>
  )
}

export default Search
