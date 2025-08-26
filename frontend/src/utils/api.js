import axios from 'axios'

const baseURL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5062/api/'

export const authAPI = axios.create({
  baseURL: baseURL + 'auth/',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

export const videoAPI = axios.create({
  baseURL: baseURL + 'videos/',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
})

export const userAPI = axios.create({
  baseURL: baseURL + 'users/',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

export const tokenManager = {
  getToken: () => localStorage.getItem('token'),
  getRefreshToken: () => localStorage.getItem('refreshToken'),
  setTokens: (token, refreshToken) => {
    localStorage.setItem('token', token)
    localStorage.setItem('refreshToken', refreshToken)
  },
  clearTokens: () => {
    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
  },
}

export const LOADING_TYPES = {
  CHECK_AUTH: 'CHECK_AUTH',
  LOGIN: 'LOGIN',
  SEND_EMAIL: 'SEND_EMAIL',
  REGISTER: 'REGISTER',
  CHECK_USERNAME: 'CHECK_USERNAME',
  CHANGE_USERNAME: 'CHANGE_USERNAME',
  LOGOUT: 'LOGOUT',
  CHECK_BIRTHDATE: 'CHECK_BIRTHDATE',
  RESET_PASSWORD: 'RESET_PASSWORD',
  SEARCH_USERS: 'SEARCH_USERS',
}

const addAuthInterceptor = (apiInstance) => {
  apiInstance.interceptors.request.use(
    (config) => {
      const token = tokenManager.getToken()
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
      return config
    },
    (error) => Promise.reject(error),
  )
}

const addRefreshInterceptor = (apiInstance, onLogout) => {
  apiInstance.interceptors.response.use(
    (response) => response,
    async (error) => {
      const originalRequest = error.config

      if (error.response?.status === 401 && !originalRequest._retry) {
        originalRequest._retry = true

        try {
          const refreshToken = tokenManager.getRefreshToken()
          if (!refreshToken) {
            onLogout?.()
            return Promise.reject(error)
          }

          const response = await authAPI.post('refresh', { refreshToken })
          const { token: newToken, refreshToken: newRefreshToken } = response.data

          if (!newToken || !newRefreshToken) {
            throw new Error('Invalid refresh response')
          }

          tokenManager.setTokens(newToken, newRefreshToken)
          originalRequest.headers.Authorization = `Bearer ${newToken}`

          return apiInstance(originalRequest)
        } catch (refreshError) {
          onLogout?.()
          return Promise.reject(refreshError)
        }
      }

      return Promise.reject(error)
    },
  )
}

export const setupAPIInterceptors = (onLogout) => {
  ;[authAPI, videoAPI, userAPI].forEach((api) => {
    addAuthInterceptor(api)
    addRefreshInterceptor(api, onLogout)
  })
}

export const handleAPIError = (error) => {
  const errorCode = error.response?.data?.errorCode || 'UNKNOWN_ERROR'
  const message = error.response?.data?.message || error.message || 'An error occurred'

  console.error('API Error:', { errorCode, message, error })

  return { errorCode, message }
}

export const parseAPIResponse = (response) => {
  const isSuccess = response.data?.isSuccess || false
  return {
    success: isSuccess,
    data: response.data,
    message: response.data?.message || '',
    errorCode: response.data?.errorCode || null,
  }
}

const apiUtils = {
  authAPI,
  videoAPI,
  userAPI,
  tokenManager,
  setupAPIInterceptors,
  handleAPIError,
  parseAPIResponse,
  LOADING_TYPES,
}

export default apiUtils
