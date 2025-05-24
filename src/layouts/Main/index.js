import classNames from 'classnames/bind'
import styles from './MainLayout.module.scss'
import Sidebar from '~/components/Sidebar'
import Drawer from '~/components/Drawer'
import { useState } from 'react'

import { useWindowWidth } from '~/hooks'

const cx = classNames.bind(styles)

function MainLayout({ children }) {
  const [selectedTooltip, setSelectedTooltip] = useState('foryou')
  const handleSelectTooltip = (key) => {
    setSelectedTooltip(key)
  }

  const windowWidth = useWindowWidth()
  const isCollapsed = windowWidth <= 1024 || selectedTooltip === 'more'
  const handleExpand = () => {
    handleSelectTooltip('foryou')
  }

  return (
    <div className={cx('MainWrapper')}>
      <Drawer onExpand={handleExpand} more={selectedTooltip === 'more'} />
      <Sidebar selectedTooltip={selectedTooltip} onSelectTooltip={handleSelectTooltip} isCollapsed={isCollapsed} />
      <div className={cx('MainContent')}>{children}</div>
    </div>
  )
}

export default MainLayout
