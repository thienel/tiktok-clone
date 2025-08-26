import { Link } from 'react-router-dom'
import classNames from 'classnames/bind'
import styles from './FixedGroup.scss'
import images from '~/assets/images/'
import Button from '~/components/Button'

const cx = classNames.bind(styles)

function FixedGroup({ isCollapsed, onSelect, searchValue }) {
  return (
    <div className={cx('GroupWrapper', { isCollapsed })}>
      <div className={cx('LogoWrapper')}>
        <Link to="/">
          <images.logoFull className={cx('LogoFull')} />
          <images.logo className={cx('Logo')} />
        </Link>
      </div>
      <div className={cx('SearchWrapper')}>
        <Button round placeholder left className={cx('animation')} onClick={() => onSelect('search')}>
          <div className={cx('SearchContent')}>
            <div className={cx('SearchIconWrapper')}>
              <images.searchIcon />
            </div>
            <span className={cx({ input: searchValue })}>{searchValue ? searchValue : 'Search'}</span>
          </div>
        </Button>
      </div>
    </div>
  )
}

export default FixedGroup
