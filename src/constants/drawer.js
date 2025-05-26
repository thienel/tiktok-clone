export const MENU_TYPE = {
  LINK: 'link',
  TOGGLE_THEME: 'toggle-theme',
  LANGUAGE: 'language',
}

export const themeID = {
  DEVICE: 'device',
  DARK: 'dark',
  LIGHT: 'light',
}

export const moreMenus = [
  { title: 'Create TikTok effects', to: '/', type: MENU_TYPE.LINK },
  {
    title: 'Creator tools',
    subItems: [
      { title: 'LIVE shopping', to: '/', type: MENU_TYPE.LINK },
      { title: 'LIVE Creator Hub', to: '/', type: MENU_TYPE.LINK },
    ],
  },
  {
    title: 'Language',
    type: MENU_TYPE.LANGUAGE,
    subItems: [
      { title: 'English (US)', to: '/', lang: 'en' },
      { title: 'Tiếng Việt', to: '/', lang: 'vi' },
    ],
  },
  {
    title: 'Dark mode',
    type: MENU_TYPE.TOGGLE_THEME,
    subItems: [
      { title: 'Use device theme', id: themeID.DEVICE },
      { title: 'Dark mode', id: themeID.DARK },
      { title: 'Light mode', id: themeID.LIGHT },
    ],
  },
  { title: 'Feedback and help', to: '/', type: MENU_TYPE.LINK },
]
