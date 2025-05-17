import classNames from 'classnames/bind'
import styles from './DefaultLayout.module.scss'
import Header from '~/components/Layout/components/Header'
import Sidebar from './Sidebar'

const ex = classNames.bind(styles)

function DefaultLayout({ children }) {
  return (
    <div className={ex('wrapper')}>
      <Header />
      <div className={ex('container')}>
        <Sidebar />
        <div className="content">{children}</div>
      </div>
    </div>
  )
}

export default DefaultLayout
