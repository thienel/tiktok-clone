import { createContext, useState, useEffect } from 'react'
import axios from 'axios'

const AuthContext = createContext()

const baseURL = process.env.REACT_APP_API_BASE_URL

const api = axios.create({
  baseURL: baseURL + 'auth/',
})

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error),
)

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  api.interceptors.response.use(
    (response) => response,
    async (error) => {
      const originalRequest = error.config

      if (originalRequest.response?.status === 400 && !originalRequest._retry) {
        originalRequest._retry = true

        try {
          const refreshToken = localStorage.getItem('refreshToken')

          if (refreshToken) {
            const response = await axios.post('refresh', { refreshToken })

            const { newToken, newRefreshToken } = response.data

            localStorage.setItem('token', newToken)
            localStorage.setItem('refreshToken', newRefreshToken)

            originalRequest.headers.Authorization = `Bearer ${newToken}`

            return api(originalRequest)
          }
        } catch (refreshError) {
          logout()
          return Promise.reject(refreshError)
        }
      }
    },
  )

  const login = async (usernameOrEmail, password) => {
    try {
      setLoading(true)
      setError('')

      const response = await api.post('login', {
        usernameOrEmail,
        password,
      })

      const { token, refreshToken, user } = response

      localStorage.setItem('token', token)
      localStorage.setItem('refreshToken', refreshToken)

      setUser(user)

      return { success: true, user }
    } catch (error) {
      const errorCode = error.response?.data?.errorCode || 'REACT_ERROR'

      setError(errorCode)
      return { success: false, message: error.response?.data?.message, errorCode }
    } finally {
      setLoading(false)
    }
  }

  const logout = () => {}

  const value = {
    user,
    loading,
    error,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
