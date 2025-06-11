import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import classNames from 'classnames/bind'
import styles from './MainLayout.module.scss'
import { useWindowWidth } from '~/hooks'
import { toggleTooltips } from '~/constants/tooltip'
import Sidebar from '~/components/Sidebar'
import Drawer from '~/components/Drawer'

const cx = classNames.bind(styles)

function MainLayout({ children }) {
  const [selectedTooltip, setSelectedTooltip] = useState('foryou')
  const prevSelectedTooltip = useRef('foryou')
  const [searchValue, setSearchValue] = useState('')

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
    if (toggleTooltips.includes(selectedTooltip) || selectedTooltip === 'messages') {
      return selectedTooltip
    }
    return false
  }, [selectedTooltip])

  const isCollapsed = useWindowWidth() <= 1024 || drawerType

  const handleExpand = useCallback(() => {
    handleSelectTooltip('foryou')
  }, [handleSelectTooltip])

  return (
    <div className={cx('MainWrapper')}>
      <Drawer onExpand={handleExpand} type={drawerType} searchValue={searchValue} setSearchValue={setSearchValue} />
      <Sidebar
        selectedTooltip={selectedTooltip}
        onSelectTooltip={handleSelectTooltip}
        isCollapsed={isCollapsed}
        searchValue={searchValue}
      />
      <main className={cx('MainContent', { expand: isCollapsed })}>{children}</main>
    </div>
  )
}

export default MainLayout
