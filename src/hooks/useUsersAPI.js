import { useCallback } from 'react'
import { userAPI, LOADING_TYPES, handleAPIError } from '~/utils/api'
import { isValidDateString } from '~/utils/validation'
import { useLoading } from '~/context/LoadingContext'

const useUsersAPI = () => {
  const { setLoading, clearLoading, isLoading } = useLoading()

  const checkUsername = useCallback(
    async (username) => {
      try {
        if (!username) {
          return {
            success: false,
            message: 'Username is required',
            errorCode: 'VALIDATION_ERROR',
          }
        }

        setLoading(LOADING_TYPES.CHECK_USERNAME)

        const response = await userAPI.post('check-username', {
          username: username.trim(),
        })

        return { success: response.data?.isSuccess || false, data: response.data }
      } catch (error) {
        const { errorCode, message } = handleAPIError(error)
        return { success: false, message, errorCode }
      } finally {
        clearLoading(LOADING_TYPES.CHECK_USERNAME)
      }
    },
    [setLoading, clearLoading],
  )

  const checkBirthdate = useCallback(
    async (birthDate) => {
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

        const response = await userAPI.post('check-birthdate', { birthDate })

        return { success: response.data?.isSuccess || false, data: response.data }
      } catch (error) {
        const { errorCode, message } = handleAPIError(error)
        return { success: false, message, errorCode }
      } finally {
        clearLoading(LOADING_TYPES.CHECK_BIRTHDATE)
      }
    },
    [setLoading, clearLoading],
  )

  const changeUsername = useCallback(
    async (email, username, type) => {
      try {
        if (!email || !username || !type) {
          return {
            success: false,
            message: 'Email, username, and type are required',
            errorCode: 'VALIDATION_ERROR',
          }
        }

        setLoading(LOADING_TYPES.CHANGE_USERNAME)

        const response = await userAPI.post('change-username', {
          username: username.trim(),
          idOrEmail: email.trim().toLowerCase(),
          type: type.trim(),
        })

        return { success: response.data?.isSuccess || false, data: response.data }
      } catch (error) {
        const { errorCode, message } = handleAPIError(error)
        return { success: false, message, errorCode }
      } finally {
        clearLoading(LOADING_TYPES.CHANGE_USERNAME)
      }
    },
    [setLoading, clearLoading],
  )

  const searchUsers = useCallback(
    async (value) => {
      try {
        if (!value) {
          return {
            success: false,
            message: 'Search value is required',
            errorCode: 'VALIDATION_ERROR',
          }
        }

        setLoading(LOADING_TYPES.SEARCH_USERS)

        const response = await userAPI.post('search', {
          value: value.trim(),
        })

        return { data: response.data.users || [] }
      } catch (error) {
        const { errorCode, message } = handleAPIError(error)
        return { success: false, message, errorCode }
      } finally {
        clearLoading(LOADING_TYPES.SEARCH_USERS)
      }
    },
    [setLoading, clearLoading],
  )

  return {
    checkUsername,
    checkBirthdate,
    changeUsername,
    searchUsers,
    isCheckingUsername: isLoading(LOADING_TYPES.CHECK_USERNAME),
    isCheckingBirthdate: isLoading(LOADING_TYPES.CHECK_BIRTHDATE),
    isChangingUsername: isLoading(LOADING_TYPES.CHANGE_USERNAME),
    isSearchingUsers: isLoading(LOADING_TYPES.SEARCH_USERS),
  }
}

export default useUsersAPI
