import { Link } from 'react-router-dom'
import classNames from 'classnames/bind'
import styles from './FixedTop.module.scss'
import images from '~/assets/images'

const cx = classNames.bind(styles)

function FixedTop({ onLogin }) {
  return (
    <div className={cx('wrapper')}>
      <div className={cx('actionBar')}>
        <Link to="/">
          <div className={cx('action')}>
            <images.getCoins />
            <span>Get Coins</span>
          </div>
        </Link>
        <Link to="/">
          <div className={cx('action')}>
            <images.getApp />
            <span>Get App</span>
          </div>
        </Link>
        <Link to="/">
          <div className={cx('action')}>
            <images.pcApp height={14} width={14} />
            <span>PC App</span>
          </div>
        </Link>
        <div className={cx('divider')} />
        <div onClick={onLogin}>
          <div className={cx('login')}>Log in</div>
        </div>
      </div>
    </div>
  )
}

export default FixedTop
