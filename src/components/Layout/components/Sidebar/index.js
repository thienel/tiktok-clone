import classNames from 'classnames/bind'

import styles from './Sidebar.module.scss'
import Button from '~/components/Button'
import MainNav from './MainNav'
import Group from './Group'
import Footer from './Footer'

const cx = classNames.bind(styles)

function Sidebar() {
  return (
    <div className={cx('SidenavContainer')}>
      <Group />
      <div className={cx('NavWrapper')}>
        <MainNav />
        <div className={cx('LoginButtonWrapper')}>
          <Button primary>Log in</Button>
        </div>
        <Footer />
      </div>
    </div>
  )
}

export default Sidebar
