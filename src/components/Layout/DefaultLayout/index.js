import classNames from 'classnames/bind'
import styles from './DefaultLayout.module.scss'
import Sidebar from '~/components/Layout/components/Sidebar'

const cx = classNames.bind(styles)

function DefaultLayout({ children }) {
  return (
    <div className={cx('wrapper')}>
      <Sidebar />
      <div className={cx('content')}>{children}</div>
    </div>
  )
}

export default DefaultLayout
