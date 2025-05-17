import { default as HomePage } from '~/pages/Home'
import { default as FollowingPage } from '~/pages/Following'
import { default as UploadPage } from '~/pages/Upload'
import { HeaderOnly } from '~/components/Layout'

export const publicRoutes = [
  { path: '/', component: HomePage },
  { path: '/following', component: FollowingPage },
  { path: '/upload', component: UploadPage, layout: HeaderOnly },
]

export const privateRoutes = []
