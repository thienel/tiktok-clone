const tooltipImages = require.context('!!@svgr/webpack!./tooltip', false, /\.svg$/)

const tooltips = {}

tooltipImages.keys().forEach((key) => {
  const fileName = key.replace('./', '').replace('.svg', '')
  tooltips[fileName] = tooltipImages(key).default
})

const images = {
  logo: require('./logo.svg').default,
  searchIcon: require('./searchIcon.svg').default,
  tooltips,
}

export default images
