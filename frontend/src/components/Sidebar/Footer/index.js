import { useState } from 'react'
import { Link } from 'react-router-dom'
import classNames from 'classnames/bind'
import styles from './Footer.scss'
import { footerSections } from '~/constants/footer'

const cx = classNames.bind(styles)

function Footer({ isCollapsed }) {
  const [selectedFooter, setSelectedFooter] = useState('')

  const handleSelectFooter = (key) => {
    if (selectedFooter !== key) {
      setSelectedFooter(key)
    } else {
      setSelectedFooter('')
    }
  }

  return (
    <div className={cx('SubNavWrapper', { isCollapsed })}>
      <div className={cx('FooterWrapper')}>
        {footerSections.map(({ key, title, links }) => (
          <div key={key}>
            <h4 onClick={() => handleSelectFooter(key)} className={cx({ isFocused: selectedFooter === key })}>
              {title}
            </h4>
            {selectedFooter === key && (
              <div className={cx('LinkWrapper')}>
                {links.map(({ label, url }) => (
                  <Link key={label} to={url}>
                    {label}
                  </Link>
                ))}
              </div>
            )}
          </div>
        ))}
        <span>&copy; 2025 TikTok</span>
      </div>
    </div>
  )
}

export default Footer
