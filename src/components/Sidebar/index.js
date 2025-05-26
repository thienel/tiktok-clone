import classNames from 'classnames/bind'

import styles from './Sidebar.module.scss'
import Button from '~/components/Button'
import MainNav from './MainNav'
import FixedGroup from './FixedGroup'
import Footer from './Footer'

const cx = classNames.bind(styles)

function Sidebar({ selectedTooltip, onSelectTooltip, isCollapsed }) {
  return (
    <div className={cx('SideNavContainer', { isCollapsed })}>
      <FixedGroup isCollapsed={isCollapsed} onSelect={onSelectTooltip} />
      <div className={cx('NavWrapper')}>
        <MainNav onSelect={onSelectTooltip} selectedTooltip={selectedTooltip} isCollapsed={isCollapsed} />
        <div className={cx('LoginButtonWrapper')}>
          <Button primary>Log in</Button>
        </div>
        <Footer isCollapsed={isCollapsed} />
      </div>
    </div>
  )
}

export default Sidebar
