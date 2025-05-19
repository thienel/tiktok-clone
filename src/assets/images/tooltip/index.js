const tooltipImages = require.context('!!@svgr/webpack!./', false, /\.svg$/)

const tooltipItems = [
  { key: 'foryou', label: 'For You', size: 32 },
  { key: 'explore', label: 'Explore', size: 32 },
  { key: 'following', label: 'Following', size: 24 },
  { key: 'upload', label: 'Upload', size: 24 },
  { key: 'live', label: 'LIVE', size: 32 },
  { key: 'profile', label: 'Profile', size: 24 },
  { key: 'more', label: 'More', size: 24 },
]

const tooltips = {}

tooltipImages.keys().forEach((key) => {
  const fileName = key.replace('./', '').replace('.svg', '')
  tooltips[fileName] = tooltipImages(key).default
})

export { tooltipItems, tooltips }
