import { Link } from 'react-router-dom'
import classNames from 'classnames/bind'
import styles from './Button.module.scss'

const cx = classNames.bind(styles)

function Button({
  children,
  to,
  href,
  primary,
  disabled,
  small,
  large,
  left,
  right,
  selected,
  round,
  placeholder,
  className,
  ...props
}) {
  var Component = 'button'
  if (to) {
    Component = Link
  } else if (href) {
    Component = 'a'
  }

  const classes = {
    [className]: className,
    primary,
    disabled,
    small,
    large,
    left,
    right,
    selected,
    round,
    placeholder,
  }

  if (disabled) {
    Object.keys(props).forEach((key) => {
      if (key.startsWith('on') && typeof (props[key] === 'function')) {
        delete props[key]
      }
    })
  }

  return (
    <Component className={cx('wrapper', classes)} {...props}>
      <span className={cx('label')}>{children}</span>
    </Component>
  )
}

export default Button
