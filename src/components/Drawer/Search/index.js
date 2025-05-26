import classNames from 'classnames/bind'
import styles from './Search.module.scss'
import CircleButton from '~/components/CircleButton'

const cx = classNames.bind(styles)

function Search({ onExpand }) {
  return (
    <div className={cx('wrapper')}>
      <div className={cx('searchWrapper')}>
        <h2 className={cx('header')}>Search</h2>
        <div className={cx('inputWrapper')}>
          <input className={cx('input')} placeholder="Search" />
        </div>
      </div>
      <div className={cx('content')}></div>
      <div className={cx('Close')}>
        <CircleButton close onClick={onExpand} />
      </div>
    </div>
  )
}

export default Search
