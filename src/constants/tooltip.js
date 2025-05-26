const tooltipImages = require.context('!!@svgr/webpack!../assets/images/tooltip', false, /\.svg$/)

const tooltipItems = [
  { key: 'foryou', label: 'For You', size: 32, focused: 'foryouFocused', url: '/' },
  { key: 'explore', label: 'Explore', size: 32, focused: 'exploreFocused', url: '/' },
  { key: 'following', label: 'Following', size: 24, focused: 'followingFocused', url: '/' },
  { key: 'upload', label: 'Upload', size: 24, focused: 'upload', url: '/' },
  { key: 'live', label: 'LIVE', size: 32, focused: 'liveFocused', url: '/' },
  { key: 'profile', label: 'Profile', size: 24, focused: 'profile', url: '/' },
  { key: 'more', label: 'More', size: 24, focused: 'more', url: '/' },
]

const tooltips = {}

tooltipImages.keys().forEach((key) => {
  const fileName = key.replace('./', '').replace('.svg', '')
  tooltips[fileName] = tooltipImages(key).default
})

const toggleTooltips = ['search', 'activity', 'more']
export { tooltipItems, tooltips, toggleTooltips }
