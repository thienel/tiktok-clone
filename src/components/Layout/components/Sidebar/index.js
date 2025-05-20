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
      <div className={cx('GroupWapper')}>
        <div className={cx('LogoWrapper')}>
          <a>
            <images.logo />
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
          {tooltipItems.map(({ key, label, size }) => {
            const IconComponent = tooltips[key]
            return (
              <div className={cx('TooltipWrapper')} key={key}>
                  <button onClick={() => handleSelect(key)}>
                    <div
                      className={cx('TooltipContent')}
                      style={{ color: selectedTooltip === key ? 'var(--color-primary)' : 'black' }}
                    >
                      <div className={cx('TooltipIconWrapper')} style={{fontSize: size}}>
                        <IconComponent/>
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
