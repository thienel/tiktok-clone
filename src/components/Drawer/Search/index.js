import classNames from 'classnames/bind'
import styles from './Search.module.scss'
import CircleButton from '~/components/CircleButton'
import images from '~/assets/images'
import { useEffect, useRef, useState } from 'react'
import { useDebounce } from '~/hooks'
import { useUsersAPI } from '~/hooks'

const cx = classNames.bind(styles)

function Search({ onExpand, searchValue, setSearchValue }) {
  const inputRef = useRef()
  const clearRef = useRef()
  const loadingRef = useRef()
  const [isFocused, setIsFocused] = useState(false)
  const debouncedSearchValue = useDebounce(searchValue, 500)
  const [searchResult, setSearchResult] = useState([])
  const { searchUsers, isSearchingUsers } = useUsersAPI()

  useEffect(() => {
    if (!debouncedSearchValue.trim()) {
      setSearchResult([])
      return
    }

    const fetch = async () => {
      const result = await searchUsers(debouncedSearchValue)
      setSearchResult(result.data || [])
    }

    fetch()
  }, [debouncedSearchValue, searchUsers])

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
      clearRef.current.style.display = !isSearchingUsers ? 'flex' : 'none'
    }
    if (loadingRef.current) {
      loadingRef.current.style.display = isSearchingUsers ? 'flex' : 'none'
    }
  }, [isSearchingUsers])

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
                        {item.name}{' '}
                        {item.isVerified && (
                          <span className={cx('verifyWrapper')}>
                            <images.verify />
                          </span>
                        )}
                      </h4>
                      <p>{item.username}</p>
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
