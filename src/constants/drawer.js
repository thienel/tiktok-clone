export const themeID = {
  DEVICE: 'device',
  DARK: 'dark',
  LIGHT: 'light',
}

export const moreMenus = [
  { title: 'Create TikTok effects', to: '/' },
  {
    title: 'Creator tools',
    subItems: [
      { title: 'LIVE shopping', to: '/' },
      { title: 'LIVE Creator Hub', to: '/' },
    ],
  },
  {
    title: 'English (US)',
    subItems: [
      { title: 'English (US)', to: '/' },
      { title: 'Tiếng Việt', to: '/' },
    ],
  },
  {
    title: 'Dark mode',
    subItems: [
      { title: 'Use device theme', id: themeID.DEVICE },
      {
        title: 'Dark mode',
        id: themeID.DARK,
      },
      {
        title: 'Light mode',
        id: themeID.LIGHT,
      },
    ],
  },
  { title: 'Feedback and help', to: '/' },
]
