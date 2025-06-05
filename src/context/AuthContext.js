import { createContext, useState, useEffect } from 'react'
import axios from 'axios'

export const AuthContext = createContext()
export default AuthContext

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
  const [loading, setLoading] = useState('')
  const [error, setError] = useState('')

  const LOADING_TYPE = {
    CHECK_AUTH: 'CHECK_AUTH',
    LOGIN: 'LOGIN',
    SEND_EMAIL: 'SEND_EMAIL',
    REGISTER: 'REGISTER',
  }

  api.interceptors.response.use(
    (response) => response,
    async (error) => {
      const originalRequest = error.config

      if (error.response?.status === 400 && !originalRequest._retry) {
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
    (error) => Promise.reject(error),
  )

  useEffect(() => {
    checkAuth()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const checkAuth = async () => {
    try {
      setLoading(LOADING_TYPE.CHECK_AUTH)
      const token = localStorage.getItem('token')
      if (token) {
        const response = await api.get('me')
        setUser(response.data.user)
      }
    } catch {
      localStorage.removeItem('token')
      localStorage.removeItem('refreshToken')
    } finally {
      setLoading('')
    }
  }

  const login = async (usernameOrEmail, password) => {
    try {
      setLoading(LOADING_TYPE.LOGIN)
      setError('')

      const response = await api.post('login', {
        usernameOrEmail,
        password,
      })

      const { token, refreshToken, user } = response.data
      localStorage.setItem('token', token)
      localStorage.setItem('refreshToken', refreshToken)

      setUser(user)

      return { success: true, user }
    } catch (error) {
      const errorCode = error.response?.errorCode || 'REACT_ERROR'

      setError(errorCode)
      return { success: false, message: error.response?.message, errorCode }
    } finally {
      setLoading('')
    }
  }

  const logout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
    setUser(null)
  }

  const register = async (email, password, birthDate, verificationCode) => {
    try {
      setLoading(LOADING_TYPE.REGISTER)
      setError('')
      const response = await api.post('register', { email, password, birthDate, verificationCode })

      if (response.data.errorCode) setError(response.data.errorCode)

      return { success: response.data.isSuccess }
    } catch {
      return { success: false }
    } finally {
      setLoading('')
    }
  }

  const sendEmailVerification = async (email) => {
    try {
      setLoading(LOADING_TYPE.SEND_EMAIL)
      setError('')

      const response = await api.post('send-verification-code', { email })

      if (response.data.errorCode) setError(response.data.errorCode)

      return { success: response.data.isSuccess }
    } catch {
      return { success: false }
    } finally {
      setLoading(false)
    }
  }

  const value = {
    user,
    loading,
    error,
    login,
    logout,
    register,
    sendEmailVerification,
    isAuthenticated: !!user,
    api,
    LOADING_TYPE,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
