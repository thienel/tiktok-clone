import axios from 'axios'

const request = axios.create({
  baseURL: 'https://tiktok.fullstack.edu.vn/api/',
})

export const get = async (path, query = {}) => {
  const respone = await request.get(path, { params: query })
  return respone.data
}

export default request
