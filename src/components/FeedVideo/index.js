import classNames from 'classnames/bind'
import styles from './FeedVideo.module.scss'

const cx = classNames.bind(styles)

function FeedVideo() {
  return (
    <article className={cx('wrapper')}>
      <div className={cx('container')}>
        <div className={cx('content')}>
          <section className={cx('video-wrapper')}>
            <div className={cx('player-container')}>
              <video className={cx('video')} src="test-video.mp4" controls autoPlay loop muted />

              {/* <video
                  class={cx('video')}
                  x5-playsinline="true"
                  webkit-playsinline="true"
                  mediatype="video"
                  data-index="-1"
                  preload="auto"
                  src="test-video-mp4"
                /> */}
            </div>
          </section>
        </div>
      </div>
      <aside className={cx('actionBar')}></aside>
    </article>
  )
}

export default FeedVideo
