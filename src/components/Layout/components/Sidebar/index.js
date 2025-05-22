import classNames from 'classnames/bind'
import { useState } from 'react'

import styles from './Sidebar.module.scss'
import Button from '~/components/Button'
import MainNav from './MainNav'
import FixedGroup from './FixedGroup'
import Footer from './Footer'
import Drawer from './Drawer'
import { useWindowWidth } from '~/hooks'

const cx = classNames.bind(styles)

function Sidebar() {
  const [selectedTooltip, setSelectedTooltip] = useState('foryou')
  const handleSelect = (key) => {
    console.log(key)
    setSelectedTooltip(key)
  }

  const windowWidth = useWindowWidth()
  const isCollapsed = windowWidth <= 1024 || selectedTooltip === 'more'
  const isExpanded = windowWidth >= 1025 && selectedTooltip !== 'more'

  return (
    <>
      <Drawer more={selectedTooltip === 'more'} />
      <div className={cx('SidenavContainer', { collapse: isCollapsed, expand: isExpanded })}>
        <FixedGroup collapse={isCollapsed} expand={isExpanded} />
        <div className={cx('NavWrapper')}>
          <MainNav onSelect={handleSelect} selected={selectedTooltip} collapse={isCollapsed} expand={isExpanded} />
          <div className={cx('LoginButtonWrapper')}>
            <Button primary>Log in</Button>
          </div>
          <Footer collapse={isCollapsed} />
        </div>
      </div>
    </>
  )
}

export default Sidebar
