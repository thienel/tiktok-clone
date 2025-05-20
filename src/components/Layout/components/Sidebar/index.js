import { useState } from 'react'
import classNames from 'classnames/bind'
import styles from './Sidebar.module.scss'
import images from '~/assets/images/'
import { tooltipItems, tooltips } from '~/assets/images/tooltip'

const cx = classNames.bind(styles)

function Sidebar() {
  const [selectedTooltip, setSelectedTooltip] = useState('foryou')
  const [selectedFooter, setSelectedFooter] = useState('')

  const handleSelect = (key) => {
    setSelectedTooltip(key)
  }

  const handleSelectFooter = (key) => {
    if (selectedFooter !== key) {
      setSelectedFooter(key)
    } else {
      setSelectedFooter('')
    }
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
            <h4
              onClick={() => handleSelectFooter('Company')}
              className={cx({ isFocused: selectedFooter === 'Company' })}
            >
              Company
            </h4>
            {selectedFooter === 'Company' && (
              <div className={cx('LinkWrapper')}>
                <a href="/">About</a>
                <a href="/">Newsroom</a>
                <a href="/">Contact</a>
                <a href="/">Careers</a>
              </div>
            )}
            <h4
              onClick={() => handleSelectFooter('Program')}
              className={cx({ isFocused: selectedFooter === 'Program' })}
            >
              Program
            </h4>
            {selectedFooter === 'Program' && (
              <div className={cx('LinkWrapper')}>
                <a href="/">TikTok for Good</a>
                <a href="/">Advertise</a>
                <a href="/">TikTok LIVE Creator Networks</a>
                <a href="/">Developers</a>
                <a href="/">Transparency</a>
                <a href="/">TikTok Rewards</a>
                <a href="/">TikTok Embeds</a>
              </div>
            )}

            <h4
              onClick={() => handleSelectFooter('policies')}
              className={cx({ isFocused: selectedFooter === 'policies' })}
            >
              Terms & Policies
            </h4>
            {selectedFooter === 'policies' && (
              <div className={cx('LinkWrapper')}>
                <a href="/">Help</a>
                <a href="/">Safety</a>
                <a href="/">Terms</a>
                <a href="/">Privacy Policy</a>
                <a href="/">Accessibility</a>
                <a href="/">Privacy Center</a>
                <a href="/">Creator Academy</a>
                <a href="/">Community Guidelines</a>
              </div>
            )}

            <span>&copy; 2025 TikTok</span>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Sidebar
