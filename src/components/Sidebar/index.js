import classNames from 'classnames/bind'
import { useState } from 'react'
import styles from './Sidebar.module.scss'
import Button from '~/components/Button'
import MainNav from './MainNav'
import FixedGroup from './FixedGroup'
import Footer from './Footer'
import LoginModal from '../LoginModal'
import { useAuth } from '~/hooks'

const cx = classNames.bind(styles)

function Sidebar({ selectedTooltip, onSelectTooltip, isCollapsed, searchValue }) {
  const { isAuthenticated } = useAuth()
  const [login, setLogin] = useState(false)
  return (
    <div className={cx('SideNavContainer', { isCollapsed })}>
      <FixedGroup isCollapsed={isCollapsed} onSelect={onSelectTooltip} searchValue={searchValue} />
      <div className={cx('NavWrapper')}>
        <MainNav onSelect={onSelectTooltip} selectedTooltip={selectedTooltip} isCollapsed={isCollapsed} />
        {!isAuthenticated && (
          <>
            <div className={cx('LoginButtonWrapper')}>
              <Button primary onClick={() => setLogin(true)}>
                Log in
              </Button>
            </div>
            <LoginModal onClose={() => setLogin(false)} isOpen={login} />
          </>
        )}
        <Footer isCollapsed={isCollapsed} />
      </div>
    </div>
  )
}

export default Sidebar
