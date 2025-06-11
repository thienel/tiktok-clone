const tooltipImages = require.context('!!@svgr/webpack!../assets/images/tooltip', false, /\.svg$/)

const tooltipItems = [
  { key: 'foryou', label: 'For You', size: 32, focused: 'foryouFocused', url: '/' , loginRequired: false},
  { key: 'explore', label: 'Explore', size: 32, focused: 'exploreFocused', url: '/', loginRequired: false },
  { key: 'following', label: 'Following', size: 24, focused: 'followingFocused', url: '/' , loginRequired: false},
  { key: 'friends', label: 'Friends', size: 32, focused: 'friendsFocused', url: '/', loginRequired: true},
  { key: 'upload', label: 'Upload', size: 24, focused: 'upload', url: '/' , loginRequired: false},
  { key: 'activity', label: 'Activity', size: 32, focused: 'activityFocused', url: '/', loginRequired: true },
  { key: 'messages', label: 'Messages', size: 24, focused: 'messagesFocused', url: '/', loginRequired: true},
  { key: 'live', label: 'LIVE', size: 32, focused: 'liveFocused', url: '/' , loginRequired: false},
  { key: 'profile', label: 'Profile', size: 24, focused: 'profile', url: '/' , loginRequired: false},
  { key: 'more', label: 'More', size: 24, focused: 'more', url: '/' , loginRequired: false},
]

const tooltips = {}

tooltipImages.keys().forEach((key) => {
  const fileName = key.replace('./', '').replace('.svg', '')
  tooltips[fileName] = tooltipImages(key).default
})

const toggleTooltips = ['search', 'activity', 'more']
export { tooltipItems, tooltips, toggleTooltips }
