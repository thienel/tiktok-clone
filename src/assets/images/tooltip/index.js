const tooltipImages = require.context('!!@svgr/webpack!./', false, /\.svg$/)

const tooltipItems = [
  { key: 'foryou', label: 'For You', size: 32, focused: 'foryouFocused' },
  { key: 'explore', label: 'Explore', size: 32, focused: 'exploreFocused' },
  { key: 'following', label: 'Following', size: 24, focused: 'followingFocused' },
  { key: 'upload', label: 'Upload', size: 24, focused: 'upload' },
  { key: 'live', label: 'LIVE', size: 32, focused: 'liveFocused' },
  { key: 'profile', label: 'Profile', size: 24, focused: 'profile' },
  { key: 'more', label: 'More', size: 24, focused: 'more' },
]

const tooltips = {}

tooltipImages.keys().forEach((key) => {
  const fileName = key.replace('./', '').replace('.svg', '')
  tooltips[fileName] = tooltipImages(key).default
})

export { tooltipItems, tooltips }
