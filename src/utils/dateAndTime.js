export function isValidDate(year, month, day) {
  const date = new Date(year, month - 1, day)
  return (
    date.getFullYear() === parseInt(year) && date.getMonth() === parseInt(month) - 1 && date.getDate() === parseInt(day)
  )
}

export function getAge(year, month, day) {
  const today = new Date()
  const birthDate = new Date(year, month - 1, day)

  let age = today.getFullYear() - birthDate.getFullYear()
  const m = today.getMonth() - birthDate.getMonth()

  if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
    age--
  }

  return age
}
