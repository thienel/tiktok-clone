import classNames from 'classnames/bind'
import styles from './SubmitButton.module.scss'
import images from '~/assets/images'

const cx = classNames.bind(styles)

function SubmitButton({ disabled, loading, content, onClick, className }) {
  return (
    <button
      className={cx(
        'wrapper',
        {
          disabled,
          loading,
        },
        className,
      )}
      onClick={!(disabled || loading) ? onClick : () => {}}
    >
      {content}
      <div className="loadingIcon">
        <images.loading style={{ margin: '0', width: '20', height: '20' }} fill="currentColor" />
      </div>
    </button>
  )
}

export default SubmitButton
