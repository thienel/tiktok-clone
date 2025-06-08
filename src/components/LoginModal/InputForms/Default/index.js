import classNames from 'classnames/bind'
import stylesInput from '../InputForms.module.scss'

const cxInput = classNames.bind(stylesInput)

function Default({ value, setValue, placeholder }) {
  return (
    <>
      <div className={cxInput('wrapper')}>
        <input
          value={value}
          onChange={(e) => setValue(e.target.value)}
          placeholder={placeholder}
          type="email"
          spellCheck="false"
        />
      </div>
    </>
  )
}

export default Default
