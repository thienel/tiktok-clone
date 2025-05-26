import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import classNames from 'classnames/bind'
import styles from './MainLayout.module.scss'
import Sidebar from '~/components/Sidebar'
import Drawer from '~/components/Drawer'
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

  const handleSelectTooltip = useCallback((key) => {
    setSelectedTooltip((current) => {
      if (key === current) {
        if (toggleTooltips.includes(key)) return prevSelectedTooltip.current
      }
      return key
    })
  }, [])

  const drawerType = useMemo(() => {
    if (toggleTooltips.includes(selectedTooltip)) {
      return selectedTooltip
    }
    return false
  }, [selectedTooltip])

  const windowWidth = useWindowWidth()
  const isCollapsed = windowWidth <= 1024 || drawerType

  const handleExpand = useCallback(() => {
    handleSelectTooltip('foryou')
  }, [handleSelectTooltip])

  return (
    <div className={cx('MainWrapper')}>
      <Drawer onExpand={handleExpand} type={drawerType} />
      <Sidebar selectedTooltip={selectedTooltip} onSelectTooltip={handleSelectTooltip} isCollapsed={isCollapsed} />
      <div className={cx('MainContent')}>{children}</div>
    </div>
  )
}

export default MainLayout
