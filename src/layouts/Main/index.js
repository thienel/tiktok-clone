import classNames from 'classnames/bind'
import styles from './MainLayout.module.scss'
import Sidebar from '~/components/Sidebar'
import Drawer from '~/components/Drawer'
import { useEffect, useRef, useState } from 'react'
import { toggleTooltips } from '~/constants/tooltip'
import { useWindowWidth } from '~/hooks'

const cx = classNames.bind(styles)

function MainLayout({ children }) {
  console.log('render Main Layout')
  const [selectedTooltip, setSelectedTooltip] = useState('foryou')
  const prevSelectedTooltip = useRef('foryou')

  useEffect(() => {
    if (!toggleTooltips.includes(selectedTooltip)) {
      prevSelectedTooltip.current = selectedTooltip
    }
  }, [selectedTooltip])

  const handleSelectTooltip = (key) => {
    if (selectedTooltip === key) {
      if (toggleTooltips.includes(selectedTooltip)) {
        setSelectedTooltip(prevSelectedTooltip.current)
      }
    } else {
      setSelectedTooltip(key)
    }
  }

  const windowWidth = useWindowWidth()
  const isCollapsed = windowWidth <= 1024 || toggleTooltips.includes(selectedTooltip)
  const handleExpand = () => {
    handleSelectTooltip('foryou')
  }

  return (
    <div className={cx('MainWrapper')}>
      {toggleTooltips.includes(selectedTooltip) && <Drawer onExpand={handleExpand} type={selectedTooltip} />}
      <Sidebar selectedTooltip={selectedTooltip} onSelectTooltip={handleSelectTooltip} isCollapsed={isCollapsed} />
      <div className={cx('MainContent')}>{children}</div>
    </div>
  )
}

export default MainLayout
