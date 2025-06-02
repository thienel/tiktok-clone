const loginOptionImages = require.context('!!@svgr/webpack!../assets/images/loginOptions', false, /\.svg$/)

const loginItems = [
  { key: 'facebook', url: '/', title: 'Continue with Facebook' },
  { key: 'google', url: '/', title: 'Continue with Google' },
  { key: 'line', url: '/', title: 'Continue with LINE' },
  { key: 'kakaotalk', url: '/', title: 'Continue with KaKaoTalk' },
  { key: 'apple', url: '/', title: 'Continue with Apple' },
]

const loginIconMapper = {}

loginOptionImages.keys().forEach((key) => {
  const fileName = key.replace('./', '').replace('.svg', '')
  loginIconMapper[fileName] = loginOptionImages(key).default
})

export { loginIconMapper, loginItems }
