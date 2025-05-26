import { useState } from 'react'
import classNames from 'classnames/bind'
import styles from './More.module.scss'
import Button from '~/components/Button'
import images from '~/assets/images'
import CircleButton from '~/components/CircleButton'
import { themeID, moreMenus } from '~/constants/drawer'
import { useTheme } from '~/hooks'
const cx = classNames.bind(styles)

function More({ onExpand }) {
  console.log('render more')

  const { themeSetting, handleSetTheme } = useTheme()

  const handlerMap = {
    [themeID.DEVICE]: () => handleSetTheme('device'),
    [themeID.DARK]: () => handleSetTheme('dark'),
    [themeID.LIGHT]: () => handleSetTheme('light'),
  }

  const [menuStack, setMenuStack] = useState([{ title: 'More', items: moreMenus }])
  const currentLevel = menuStack[menuStack.length - 1]

  const handleSelectMore = (item) => {
    if (item.subItems) {
      setMenuStack((prev) => [...prev, { title: item.title, items: item.subItems }])
    } else {
      item.onClick?.()
      handlerMap[item.id]?.()
    }
  }

  const handleBackButton = () => {
    if (menuStack.length > 1) {
      setMenuStack((prev) => prev.slice(0, -1))
    } else {
      onExpand()
    }
  }

  return (
    <div className={cx('MoreWrapper')}>
      <div className={cx('MoreHeader')}>
        {menuStack.length > 1 && (
          <div className={cx('Back')}>
            <CircleButton flipRTL small onClick={handleBackButton} />
          </div>
        )}
        <h2>{currentLevel.title}</h2>
      </div>
      <div className={cx('MoreContent')}>
        {currentLevel.items.map((item, index) => (
          <Button key={index} between onClick={() => handleSelectMore(item)} to={item.to}>
            {item.title}
            {item.subItems && <images.flipLTR />}
            {item.id === themeSetting && <images.checked />}
          </Button>
        ))}
      </div>
      {menuStack.length === 1 && (
        <div className={cx('Close')}>
          <CircleButton close onClick={handleBackButton} />
        </div>
      )}
    </div>
  )
}

export default More
