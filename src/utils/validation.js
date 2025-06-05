export function isValidDate(year, month, day) {
  const date = new Date(year, month - 1, day)
  return (
    date.getFullYear() === parseInt(year) && date.getMonth() === parseInt(month) - 1 && date.getDate() === parseInt(day)
  )
}

export const isValidCode = (code) => {
  if (!code) return true
  const regex = /^\d{6}$/
  return regex.test(code)
}

export const checkPasswordLength = (password) => password.length >= 8 && password.length <= 20

export const checkPasswordAcceptedChar = (password) => {
  const regex = /^[a-zA-Z0-9!@#$%^&*()_\-+=[\]{}|;:'",.<>?\\/]*$/
  return regex.test(password)
}

export const checkPasswordSpecialChar = (password) => {
  const regex = /^(?=.*\d)(?=.*[!@#$%^&*(),.?":{}|<>_\-+=\\/[\]`~]).+$/
  return regex.test(password)
}

export const isValidEmailFormat = (email) => {
  if (!email) return true
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return regex.test(email)
}
