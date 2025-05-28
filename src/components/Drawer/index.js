import { memo } from 'react'
import classNames from 'classnames/bind'
import styles from './Drawer.module.scss'
import More from './More'
import Search from './Search'

const cx = classNames.bind(styles)

function Drawer({ type, onExpand, searchValue, setSearchValue }) {
  console.log('render drawer')

  return (
    <div className={cx('Wrapper', { Open: type })}>
      {type === 'more' && <More onExpand={onExpand} />}
      {type === 'search' && <Search onExpand={onExpand} searchValue={searchValue} setSearchValue={setSearchValue} />}
    </div>
  )
}

export default memo(Drawer)
