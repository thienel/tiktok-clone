import classNames from 'classnames/bind'
import styles from './Drawer.module.scss'
import Button from '~/components/Button'
import images from '~/assets/images'
import CircleButton from '~/components/CircleButton'
import { useState } from 'react'

const cx = classNames.bind(styles)

const moreMenus = [
  { title: 'Create TikTok effects', to: '/' },
  {
    title: 'Creator tools',
    subItems: [
      { title: 'LIVE shopping', to: '/' },
      { title: 'LIVE Creator Hub', to: '/' },
    ],
  },
  {
    title: 'English (US)',
    subItems: [
      { title: 'English (US)', to: '/' },
      { title: 'Tiếng Việt', to: '/' },
    ],
  },
  {
    title: 'Dark mode',
    subItems: [
      { title: 'Use device theme', onClick: () => {} },
      { title: 'Dark mode', onClick: () => {} },
      { title: 'Light mode', onClick: () => {} },
    ],
  },
  { title: 'Feedback and help', to: '/' },
]

function Drawer({ more, message, activity, onExpand }) {
  const [moreMenuStack, setMoreMenuStack] = useState([moreMenus])

  const currentMoreMenu = moreMenuStack.length - 1

  const handleSelectMore = (item) => {
    if (item.subItems) {
      setMoreMenuStack((prev) => [...prev, item.subItems])
    } else {
      item.onClick?.()
    }
  }

  const handleBackButton = () => {
    if (moreMenuStack.length > 1) {
      setMoreMenuStack((prev) => prev.slice(0, -1))
    } else {
      onExpand()
    }
  }

  return (
    <div className={cx('Wrapper', { Open: more })}>
      <div className={cx('MoreWrapper')}>
        <div className={cx('MoreHeader')}>
          <h2>More</h2>
        </div>
        <div className={cx('MoreContent')}>
          {moreMenuStack[currentMoreMenu].map((item, index) => {
            return (
              <Button key={index} round between onClick={() => handleSelectMore(item)} to={item.to}>
                {item.title}
                {item.subItems && <images.flipLTR />}
              </Button>
            )
          })}
        </div>
        <div className={cx('Close')}>
          <CircleButton close onClick={handleBackButton} />
        </div>
      </div>
    </div>
  )
}

export default Drawer
