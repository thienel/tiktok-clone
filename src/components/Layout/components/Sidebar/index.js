import { useState } from 'react'
import classNames from 'classnames/bind'
import styles from './Sidebar.module.scss'
import images from '~/assets/images/'
import { tooltipItems, tooltips } from '~/assets/images/tooltip'

const cx = classNames.bind(styles)

function Sidebar() {
  const [selectedTooltip, setSelectedTooltip] = useState('foryou')

  const handleSelect = (key) => {
    setSelectedTooltip(key)
  }

  return (
    <div className={cx('SidenavContainer')}>
      <div className={cx('GroupWrapper')}>
        <div className={cx('LogoWrapper')}>
          <a href="/">
            <images.logoFull className={cx('LogoFull')} />
            <images.logo className={cx('Logo')} />
          </a>
        </div>
        <div className={cx('SearchWrapper')}>
          <button className={cx('SearchButton')} type="button">
            <div className={cx('SearchIconWrapper')}>
              <images.searchIcon />
            </div>
            <div className={cx('SearchLabel')}>Search</div>
          </button>
        </div>
      </div>
      <div className={cx('NavWrapper')}>
        <div className={cx('MainNavWrapper')}>
          {tooltipItems.map(({ key, label, size, focused }) => {
            const IconComponent = tooltips[key]
            const IconComponentSelected = tooltips[focused]
            const isSelected = selectedTooltip === key
            return (
              <div className={cx('TooltipWrapper')} key={key}>
                <button onClick={() => handleSelect(key)}>
                  <div
                    className={cx('TooltipContent')}
                    style={{ color: isSelected ? 'var(--color-primary)' : 'var(--color-black)' }}
                  >
                    <div className={cx('TooltipIconWrapper')} style={{ fontSize: size }}>
                      {isSelected ? <IconComponentSelected /> : <IconComponent />}
                    </div>
                    <div className={cx('TooltipLabel')}>{label}</div>
                  </div>
                </button>
              </div>
            )
          })}
        </div>
        <div className={cx('LoginButtonWrapper')}>
          <button>
            <div>Log in</div>
          </button>
        </div>
        <div className={cx('SubNavWrapper')}>
          <div className={cx('FooterWrapper')}>
            <h4>Company</h4>
            <h4>Program</h4>
            <h4>Terms & Policies</h4>
            <span>&copy; 2025 TikTok</span>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Sidebar
