import classNames from 'classnames/bind'
import styles from './FeedVideo.module.scss'
import { useWindowWidth } from '~/hooks'
import Video from '~/components/Video'

const cx = classNames.bind(styles)

function FeedVideo() {
  const windowWidth = useWindowWidth()
  const isExpand = windowWidth <= 1024

  return (
    <div className={cx('wrapper')}>
      <div className={cx('container')}>
        <article className={cx('content', { expand: isExpand })}>
          <Video />
          <section className={cx('section-actionBar')}></section>
        </article>
      </div>
      <aside className={cx('actionBar')}></aside>
    </div>
  )
}

export default FeedVideo
