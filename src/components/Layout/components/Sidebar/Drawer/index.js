import { useState } from 'react'
import classNames from 'classnames/bind'
import styles from './Drawer.module.scss'
import Button from '~/components/Button'
import images from '~/assets/images'
import CircleButton from '~/components/CircleButton'
import { themeID, moreMenus } from '~/constants/drawer'
import { useTheme } from '~/hooks'
const cx = classNames.bind(styles)

function Drawer({ more, message, activity, onExpand }) {
  const { handleSetTheme } = useTheme()

  const handlerMap = {
    [themeID.DEVICE]: () => {
      /* set device theme */
    },
    [themeID.DARK]: () => handleSetTheme('dark'),
    [themeID.LIGHT]: () => handleSetTheme('light'),
  }

  const [moreMenuStack, setMoreMenuStack] = useState([moreMenus])
  const currentMoreMenu = moreMenuStack.length - 1

  const handleSelectMore = (item) => {
    if (item.subItems) {
      setMoreMenuStack((prev) => [...prev, item.subItems])
    } else {
      item.onClick?.()
      handlerMap[item.id]?.()
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
          {moreMenuStack[currentMoreMenu].map((item, index) => (
            <Button key={index} round between onClick={() => handleSelectMore(item)} to={item.to}>
              {item.title}
              {item.subItems && <images.flipLTR />}
            </Button>
          ))}
        </div>
        <div className={cx('Close')}>
          <CircleButton close onClick={handleBackButton} />
        </div>
      </div>
    </div>
  )
}

export default Drawer
