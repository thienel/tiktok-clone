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
  between,
  selected,
  round,
  placeholder,
  className,
  ...ComponentProps
}) {
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
    primary,
    disabled,
    small,
    large,
    left,
    right,
    between,
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
      {children}
    </Component>
  )
}

export default Button
