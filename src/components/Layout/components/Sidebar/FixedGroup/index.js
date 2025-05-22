import { Link } from 'react-router-dom'
import classNames from 'classnames/bind'
import styles from './FixedGroup.scss'
import images from '~/assets/images/'
import Button from '~/components/Button'

const cx = classNames.bind(styles)

function FixedGroup({ collapse, expand }) {
  return (
    <div className={cx('GroupWrapper', { collapse, expand })}>
      <div className={cx('LogoWrapper')}>
        <Link to="/">
          <images.logoFull className={cx('LogoFull')} />
          <images.logo className={cx('Logo')} />
        </Link>
      </div>
      <div className={cx('SearchWrapper')}>
        <Button round placeholder left className={cx('collapseAnimation')}>
          <div className={cx('SearchContent')}>
            <div className={cx('SearchIconWrapper')}>
              <images.searchIcon />
            </div>
            <span>Search</span>
          </div>
        </Button>
      </div>
    </div>
  )
}

export default FixedGroup
