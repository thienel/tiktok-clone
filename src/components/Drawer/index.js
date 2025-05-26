import { memo } from 'react'
import classNames from 'classnames/bind'
import styles from './Drawer.module.scss'
import More from './More'

const cx = classNames.bind(styles)

function Drawer({ type, onExpand }) {
  console.log('render drawer')
  return <div className={cx('Wrapper', { Open: type })}>{type === 'more' && <More onExpand={onExpand} />}</div>
}

export default memo(Drawer)
