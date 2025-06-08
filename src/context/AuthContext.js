import { createContext, useState, useEffect, useCallback } from 'react'
import axios from 'axios'
import { isValidDateString } from '~/utils/validation'

export const AuthContext = createContext()
export default AuthContext

const baseURL = process.env.REACT_APP_API_BASE_URL

const api = axios.create({
  baseURL: baseURL + 'auth/',
  timeout: 10000, // 10 second timeout
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
  const [isInitialized, setIsInitialized] = useState(false)

  const LOADING_TYPE = {
    CHECK_AUTH: 'CHECK_AUTH',
    LOGIN: 'LOGIN',
    SEND_EMAIL: 'SEND_EMAIL',
    REGISTER: 'REGISTER',
    CHECK_USERNAME: 'CHECK_USERNAME',
    CHANGE_USERNAME: 'CHANGE_USERNAME',
    LOGOUT: 'LOGOUT',
    CHECK_BIRTHDATE: 'CHECK_BIRTHDATE',
  }

  const logout = useCallback(() => {
    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
    setUser(null)
    setLoading('')
  }, [])

  api.interceptors.response.use(
    (response) => response,
    async (error) => {
      const originalRequest = error.config

      if (error.response?.status === 401 && !originalRequest._retry) {
        originalRequest._retry = true

        try {
          const refreshToken = localStorage.getItem('refreshToken')
          if (!refreshToken) {
            logout()
            return Promise.reject(error)
          }

          const response = await api.post('refresh', { refreshToken })
          const { token: newToken, refreshToken: newRefreshToken } = response.data

          // Validate tokens exist
          if (!newToken || !newRefreshToken) {
            throw new Error('Invalid refresh response')
          }

          localStorage.setItem('token', newToken)
          localStorage.setItem('refreshToken', newRefreshToken)

          originalRequest.headers.Authorization = `Bearer ${newToken}`
          return api(originalRequest)
        } catch (refreshError) {
          logout()
          return Promise.reject(refreshError)
        }
      }

      return Promise.reject(error)
    },
  )

  useEffect(() => {
    checkAuth()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const checkAuth = async () => {
    try {
      setLoading(LOADING_TYPE.CHECK_AUTH)
      const token = localStorage.getItem('token')

      if (!token) {
        setIsInitialized(true)
        return
      }

      const response = await api.get('me')

      if (response.data?.user) {
        setUser(response.data.user)
      } else {
        // Invalid response structure
        throw new Error('Invalid user data')
      }
    } catch (error) {
      console.error('Auth check failed:', error)
      localStorage.removeItem('token')
      localStorage.removeItem('refreshToken')
      setUser(null)
    } finally {
      setLoading('')
      setIsInitialized(true)
    }
  }

  const login = async (usernameOrEmail, password) => {
    try {
      if (!usernameOrEmail || !password) {
        return {
          success: false,
          message: 'Username/email and password are required',
          errorCode: 'VALIDATION_ERROR',
        }
      }

      setLoading(LOADING_TYPE.LOGIN)

      const response = await api.post('login', {
        usernameOrEmail: usernameOrEmail.trim(),
        password,
      })

      const { token, refreshToken, user } = response.data

      if (!token || !refreshToken || !user) {
        throw new Error('Invalid login response')
      }

      localStorage.setItem('token', token)
      localStorage.setItem('refreshToken', refreshToken)
      setUser(user)

      return { success: true, user }
    } catch (error) {
      console.error('Login error:', error)
      const errorCode = error.response?.data?.errorCode || 'LOGIN_ERROR'
      const message = error.response?.data?.message || error.message || 'Login failed'

      return { success: false, message, errorCode }
    } finally {
      setLoading('')
    }
  }

  const register = async (email, password, birthDate, verificationCode) => {
    try {
      if (!email || !password || !birthDate || !verificationCode) {
        return {
          success: false,
          message: 'All fields are required',
          errorCode: 'VALIDATION_ERROR',
        }
      }

      setLoading(LOADING_TYPE.REGISTER)

      const response = await api.post('register', {
        email: email.trim().toLowerCase(),
        password,
        birthDate,
        verificationCode,
      })

      return { success: true, data: response.data }
    } catch (error) {
      console.error('Registration error:', error)
      const errorCode = error.response?.data?.errorCode || 'REGISTRATION_ERROR'
      const message = error.response?.data?.message || error.message || 'Registration failed'

      return { success: false, message, errorCode }
    } finally {
      setLoading('')
    }
  }

  const sendEmailVerification = async (email) => {
    try {
      if (!email) {
        return {
          success: false,
          message: 'Email is required',
          errorCode: 'VALIDATION_ERROR',
        }
      }

      setLoading(LOADING_TYPE.SEND_EMAIL)

      const response = await api.post('send-verification-code', {
        email: email.trim().toLowerCase(),
      })

      return { success: response.data?.isSuccess || false, data: response.data }
    } catch (error) {
      console.error('Send email error:', error)
      const errorCode = error.response?.data?.errorCode || 'EMAIL_ERROR'
      const message = error.response?.data?.message || error.message || 'Failed to send email'

      return { success: false, message, errorCode }
    } finally {
      setLoading('')
    }
  }

  const checkUsername = async (username) => {
    try {
      if (!username) {
        return {
          success: false,
          message: 'Username is required',
          errorCode: 'VALIDATION_ERROR',
        }
      }

      setLoading(LOADING_TYPE.CHECK_USERNAME)

      const response = await api.post('check-username', {
        username: username.trim(),
      })

      return { success: response.data?.isSuccess || false, data: response.data }
    } catch (error) {
      console.error('Check username error:', error)
      const errorCode = error.response?.data?.errorCode || 'USERNAME_CHECK_ERROR'
      const message = error.response?.data?.message || error.message || 'Failed to check username'

      return { success: false, message, errorCode }
    } finally {
      setLoading('')
    }
  }

  const checkBirthdate = async (birthDate) => {
    try {
      birthDate = birthDate.trim()
      if (!birthDate || !isValidDateString(birthDate)) {
        return {
          success: false,
          message: 'BirthDate is required',
          errorCode: 'VALIDATION_ERROR',
        }
      }

      setLoading(LOADING_TYPE.CHECK_BIRTHDATE)

      const response = await api.post('check-birthdate', { birthDate })

      return { success: response.data?.isSuccess || false, data: response.data }
    } catch (error) {
      console.error('Check birthdate error:', error)
      const errorCode = error.response?.data?.errorCode || 'BIRTHDATE_CHECK_ERROR'
      const message = error.response?.data?.message || error.message || 'Failed to check birthdate'

      return { success: false, message, errorCode }
    } finally {
      setLoading('')
    }
  }

  const changeUsername = async (email, username) => {
    try {
      if (!email || !username) {
        return {
          success: false,
          message: 'Email and username are required',
          errorCode: 'VALIDATION_ERROR',
        }
      }

      setLoading(LOADING_TYPE.CHANGE_USERNAME)

      const response = await api.post('change-username', {
        username: username.trim(),
        email: email.trim().toLowerCase(),
      })

      return { success: response.data?.isSuccess || false, data: response.data }
    } catch (error) {
      console.error('Change username error:', error)
      const errorCode = error.response?.data?.errorCode || 'USERNAME_CHANGE_ERROR'
      const message = error.response?.data?.message || error.message || 'Failed to change username'

      return { success: false, message, errorCode }
    } finally {
      setLoading('')
    }
  }

  const value = {
    user,
    loading,
    isInitialized,
    login,
    logout,
    register,
    sendEmailVerification,
    checkUsername,
    changeUsername,
    checkAuth,
    checkBirthdate,
    isAuthenticated: !!user,
    api,
    LOADING_TYPE,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
