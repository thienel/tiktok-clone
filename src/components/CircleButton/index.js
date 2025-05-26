import { Link } from 'react-router-dom'
import classNames from 'classnames/bind'
import styles from './CircleButton.module.scss'
import images from '~/assets/images'

const cx = classNames.bind(styles)

function CircleButton({ to, href, close, flipLTR, flipRTL, small, className, ...ComponentProps }) {
  const props = { ...ComponentProps }

  var Component = 'button'
  if (to) {
    Component = Link
    props.to = to
  } else if (href) {
    Component = 'a'
    props.href = href
  }

  const classes = {
    [className]: className,
    close,
    small,
    flipLTR,
    flipRTL,
  }

  return (
    <Component className={cx('wrapper', classes)} {...props}>
      {close ? <images.close /> : flipLTR ? <images.flipLTR /> : flipRTL ? <images.flipRTL /> : ''}
    </Component>
  )
}

export default CircleButton
