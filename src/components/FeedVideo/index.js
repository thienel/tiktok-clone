import classNames from 'classnames/bind'
import styles from './FeedVideo.module.scss'

const cx = classNames.bind(styles)

function FeedVideo() {
  return (
    <article className={cx('wrapper')}>
      <div className={cx('container')}>
        <section className={cx('content')}></section>
        <section className={cx('actionBar')}></section>
      </div>
    </article>
  )
}

export default FeedVideo
