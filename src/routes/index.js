import { default as HomePage } from '~/pages/Home'
import { default as FollowingPage } from '~/pages/Following'

export const publicRoutes = [
  { path: '/', component: HomePage },
  { path: '/', component: FollowingPage },
]

export const privateRoutes = []
