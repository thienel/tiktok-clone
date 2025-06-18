import { memo, useEffect, useState } from 'react'
import classNames from 'classnames/bind'
import styles from './Drawer.module.scss'
import More from './More'
import Search from './Search'

const cx = classNames.bind(styles)

function Drawer({ type, onExpand, searchValue, setSearchValue }) {
  const [overlay, setOverlay] = useState(false)
  useEffect(() => {
    setOverlay(() => type !== 'messages')
  }, [type])

  return (
    <div className={cx({ Open: type })}>
      <div className={cx('Wrapper')}>
        {type === 'more' && <More onExpand={onExpand} />}
        {type === 'search' && <Search onExpand={onExpand} searchValue={searchValue} setSearchValue={setSearchValue} />}
      </div>
      <div className={cx({ overlay })} onClick={onExpand} />
    </div>
  )
}

export default memo(Drawer)
