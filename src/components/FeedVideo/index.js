import classNames from 'classnames/bind'
import styles from './FeedVideo.module.scss'
import { useWindowWidth } from '~/hooks'

const cx = classNames.bind(styles)

function FeedVideo() {
  const windowWidth = useWindowWidth()
  const isExpand = windowWidth <= 1024

  return (
    <div className={cx('wrapper')}>
      <div className={cx('container')}>
        <article className={cx('content', { expand: isExpand })}>
          <section className={cx('video-wrapper')}>
            <div className={cx('player-container')}>
              <video className={cx('video')} src="test-video.mp4" controls autoPlay loop muted />
            </div>
          </section>
          <section className={cx('section-actionBar')}></section>
        </article>
      </div>
      <aside className={cx('actionBar')}></aside>
    </div>
  )
}

export default FeedVideo
