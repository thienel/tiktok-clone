import { createContext, useState, useEffect, useCallback } from 'react'
import { authAPI, tokenManager, setupAPIInterceptors, handleAPIError, LOADING_TYPES } from '~/utils/api'
import { isValidDateString } from '~/utils/validation'

export const AuthContext = createContext()
export default AuthContext

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState('')
  const [isInitialized, setIsInitialized] = useState(false)

  const logout = useCallback(() => {
    tokenManager.clearTokens()
    setUser(null)
    setLoading('')
  }, [])

  // Initialize API interceptors with logout callback
  useEffect(() => {
    setupAPIInterceptors(logout)
  }, [logout])

  useEffect(() => {
    checkAuth()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const checkAuth = async () => {
    try {
      setLoading(LOADING_TYPES.CHECK_AUTH)
      const token = tokenManager.getToken()

      if (!token) {
        setIsInitialized(true)
        return
      }

      const response = await authAPI.get('me')

      if (response.data?.user) {
        setUser(response.data.user)
      } else {
        throw new Error('Invalid user data')
      }
    } catch (error) {
      console.error('Auth check failed:', error)
      tokenManager.clearTokens()
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

      setLoading(LOADING_TYPES.LOGIN)

      const response = await authAPI.post('login', {
        usernameOrEmail: usernameOrEmail.trim(),
        password,
      })

      const { token, refreshToken, user } = response.data

      if (!token || !refreshToken || !user) {
        throw new Error('Invalid login response')
      }

      tokenManager.setTokens(token, refreshToken)
      setUser(user)

      return { success: true, user }
    } catch (error) {
      const { errorCode, message } = handleAPIError(error)
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

      setLoading(LOADING_TYPES.REGISTER)

      const response = await authAPI.post('register', {
        email: email.trim().toLowerCase(),
        password,
        birthDate,
        verificationCode,
      })

      return { success: true, data: response.data }
    } catch (error) {
      const { errorCode, message } = handleAPIError(error)
      return { success: false, message, errorCode }
    } finally {
      setLoading('')
    }
  }

  const sendEmailVerification = async (email, type = '') => {
    try {
      if (!email) {
        return {
          success: false,
          message: 'Email is required',
          errorCode: 'VALIDATION_ERROR',
        }
      }

      setLoading(LOADING_TYPES.SEND_EMAIL)

      const response = await authAPI.post('send-verification-code', {
        email: email.trim().toLowerCase(),
        type,
      })

      return { success: response.data?.isSuccess || false, data: response.data }
    } catch (error) {
      const { errorCode, message } = handleAPIError(error)
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

      setLoading(LOADING_TYPES.CHECK_USERNAME)

      const response = await authAPI.post('check-username', {
        username: username.trim(),
      })

      return { success: response.data?.isSuccess || false, data: response.data }
    } catch (error) {
      const { errorCode, message } = handleAPIError(error)
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

      setLoading(LOADING_TYPES.CHECK_BIRTHDATE)

      const response = await authAPI.post('check-birthdate', { birthDate })

      return { success: response.data?.isSuccess || false, data: response.data }
    } catch (error) {
      const { errorCode, message } = handleAPIError(error)
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

      setLoading(LOADING_TYPES.CHANGE_USERNAME)

      const response = await authAPI.post('change-username', {
        username: username.trim(),
        email: email.trim().toLowerCase(),
      })

      return { success: response.data?.isSuccess || false, data: response.data }
    } catch (error) {
      const { errorCode, message } = handleAPIError(error)
      return { success: false, message, errorCode }
    } finally {
      setLoading('')
    }
  }

  const resetPassword = async (email, password, verificationCode) => {
    try {
      if (!email || !password || !verificationCode) {
        return {
          success: false,
          message: 'All fields are required',
          errorCode: 'VALIDATION_ERROR',
        }
      }

      setLoading(LOADING_TYPES.RESET_PASSWORD)

      const response = await authAPI.post('reset-password', {
        email: email.trim(),
        password: password.trim(),
        verificationCode: verificationCode.trim(),
      })

      return { success: response.data?.isSuccess || false, data: response.data }
    } catch (error) {
      const { errorCode, message } = handleAPIError(error)
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
    resetPassword,
    isAuthenticated: !!user,
    LOADING_TYPES,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}