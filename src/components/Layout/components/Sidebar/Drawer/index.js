import classNames from 'classnames/bind'
import styles from './Drawer.module.scss'
import Button from '~/components/Button'
import images from '~/assets/images'
import CircleButton from '~/components/CircleButton'

const cx = classNames.bind(styles)

function Drawer({ more, message, activity }) {
  return (
    <div className={cx('Wrapper', { Open: more })}>
      <div className={cx('MoreWrapper')}>
        <div className={cx('MoreHeader')}>
          <h2>More</h2>
        </div>
        <div className={cx('MoreContent')}>
          <Button round between>
            Create TikTok effects
          </Button>
          <Button round between>
            Creator tools
            <images.flipLTR />
          </Button>
          <Button round between>
            English (US)
            <images.flipLTR />
          </Button>
          <Button round between>
            Dark mode
            <images.flipLTR />
          </Button>
          <Button round between>
            Feedback and help
          </Button>
        </div>
        <div className={cx('Close')}>
          <CircleButton close />
        </div>
      </div>
    </div>
  )
}

export default Drawer
