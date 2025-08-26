import { useRef, useState } from 'react'
import classNames from 'classnames/bind'
import styles from './Video.module.scss'
import Icon from '~/assets/images/video'

const cx = classNames.bind(styles)

function Video() {
  const [isMuted, setIsMuted] = useState(true)
  const videoRef = useRef(null)

  const handleSoundToggle = () => {
    if (videoRef.current) {
      videoRef.current.muted = !videoRef.current.muted
      setIsMuted(videoRef.current.muted)
    }
  }

  return (
    <section className={cx('video-wrapper')}>
      <div className={cx('player-container')}>
        <video ref={videoRef} className={cx('video')} src="test-video.mp4" controls autoPlay loop muted />
      </div>
      <div className={cx('media-top')}>
        <div className={cx('button-wrapper')}>
          <button className={cx('sound-button')} onClick={handleSoundToggle}>
            {isMuted ? <Icon.SoundOff /> : <Icon.SoundOn />}
          </button>
        </div>
        <div className={cx('button-wrapper')}>
          <button className={cx('menu-button')}>
            <Icon.More width={24} height={24} />
          </button>
        </div>
      </div>
    </section>
  )
}

export default Video
