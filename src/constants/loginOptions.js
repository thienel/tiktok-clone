const loginOptionImages = require.context('!!@svgr/webpack!../assets/images/loginOptions', false, /\.svg$/)

const loginItems = [
  { key: 'qr', url: '/' },
  { key: 'username' },
  { key: 'facebook', url: '/' },
  { key: 'google', url: '/' },
  { key: 'line', url: '/' },
  { key: 'kakaotalk', url: '/' },
  { key: 'apple', url: '/' },
]

const loginIconMapper = {}

loginOptionImages.keys().forEach((key) => {
  const fileName = key.replace('./', '').replace('.svg', '')
  loginIconMapper[fileName] = loginOptionImages(key).default
})

export { loginIconMapper, loginItems }
