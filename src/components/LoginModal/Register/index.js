import classNames from 'classnames/bind'
import styles from './Register.module.scss'
import images from '~/assets/images'

const cx = classNames.bind(styles)

function Register({ open, onBack }) {
  return (
    <div className={cx('wrapper', { open })}>
      <h2 className={cx('title')}>Sign up</h2>
      <div className={cx('title-birthday')}>When’s your birthday?</div>
      <div className={cx('age-selector')}>
        <div className={cx('selector')}>
          Month
          <div className={cx('iconwrapper')}>
            <images.selector />
          </div>
        </div>
        <div className={cx('selector')}>
          Day
          <div className={cx('iconwrapper')}>
            <images.selector />
          </div>
        </div>
        <div className={cx('selector')}>
          Year
          <div className={cx('iconwrapper')}>
            <images.selector />
          </div>
        </div>
      </div>
      <span className={cx('age-validation')}>Your birthday won't be shown publicly.</span>
      <div className={cx('title-method')}>
        Phone <span onClick={() => {}}>Sign up with email</span>
      </div>
    </div>
  )
}

export default Register
