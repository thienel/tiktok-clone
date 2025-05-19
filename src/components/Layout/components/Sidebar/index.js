import { useState } from 'react'
import classNames from 'classnames/bind'
import styles from './Sidebar.module.scss'
import images from '~/assets/images/'

const cx = classNames.bind(styles)

const tooltipItems = [
  { key: 'foryou', label: 'For You', size: 32 },
  { key: 'explore', label: 'Explore', size: 32 },
  { key: 'following', label: 'Following', size: 24 },
  { key: 'upload', label: 'Upload', size: 24 },
  { key: 'live', label: 'LIVE', size: 32 },
  { key: 'profile', label: 'Profile', size: 24 },
  { key: 'more', label: 'More', size: 24 },
]

function Sidebar() {
  const [selectedTooltip, setSelectedTooltip] = useState('foryou')

  const handleSelect = (key) => {
    setSelectedTooltip(key)
  }

  return (
    <div className={cx('SidenavContainer')}>
      <div className={cx('GroupWapper')}>
        <div className={cx('LogoWrapper')}>
          <a>
            <img src={images.logo} alt="TikTok" />
          </a>
        </div>
        <div className={cx('SearchWrapper')}>
          <button className={cx('SearchButton')} type="button">
            <div className={cx('SearchIconWrapper')}>
              <img src={images.searchIcon} alt="" />
            </div>
            <div className={cx('SearchLabel')}>Search</div>
          </button>
        </div>
      </div>
      <div className={cx('NavWrapper')}>
        <div className={cx('MainNavWrapper')}>
          {tooltipItems.map(({ key, label, size }) => {
            const IconComponent = images.tooltips[key]
            return (
              <div className={cx('TooltipWrapper')} key={key}>
                <a>
                  <button onClick={() => handleSelect(key)}>
                    <div
                      className={cx('TooltipContent')}
                      style={{ color: selectedTooltip === key ? 'var(--color-primary)' : 'black' }}
                    >
                      <div className={cx('TooltipIconWrapper')}>
                        <IconComponent height={size} />
                      </div>
                      <div className={cx('TooltipLabel')}>{label}</div>
                    </div>
                  </button>
                </a>
              </div>
            )
          })}
        </div>
        <div className={cx('SubMainNavContentContainer')}>{/**sub nav content */}</div>
        <div className={cx('SubMainNavFooterContainer')}>{/**sub nav footer */}</div>
      </div>
    </div>
  )
}

export default Sidebar
