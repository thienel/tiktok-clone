import classNames from 'classnames/bind'
import { tooltipItems, tooltips } from '~/constants/tooltip'
import styles from './MainNav.module.scss'
import Button from '~/components/Button'

const cx = classNames.bind(styles)

function MainNav({ onSelect, selected, collapse, expand }) {
  return (
    <div className={cx('MainNavWrapper')}>
      {tooltipItems.map(({ key, label, size, focused, url }) => {
        const Icon = tooltips[key]
        const IconSelected = tooltips[focused]
        const isSelected = selected === key
        return (
          <Button key={key} to={url} onClick={() => onSelect(key)} selected={isSelected} left>
            <div className={cx('TooltipContent')}>
              <div className={cx('TooltipIconWrapper')} style={{ fontSize: size }}>
                {isSelected ? <IconSelected /> : <Icon />}
              </div>
              <span className={cx('animation', { collapse, expand })}>{label}</span>
            </div>
          </Button>
        )
      })}
    </div>
  )
}

export default MainNav
