import { createContext, useState, useContext, useCallback } from 'react'
import { LOADING_TYPES } from '~/utils/api'

const LoadingContext = createContext()

export const useLoading = () => {
  const context = useContext(LoadingContext)
  if (!context) {
    throw new Error('useLoading must be used within a LoadingProvider')
  }
  return context
}

export const LoadingProvider = ({ children }) => {
  const [loadingStates, setLoadingStates] = useState(new Set())

  const setLoading = useCallback((type) => {
    setLoadingStates((prev) => new Set([...prev, type]))
  }, [])

  const clearLoading = useCallback((type) => {
    setLoadingStates((prev) => {
      const newSet = new Set(prev)
      newSet.delete(type)
      return newSet
    })
  }, [])

  const clearAllLoading = useCallback(() => {
    setLoadingStates(new Set())
  }, [])

  const isLoading = useCallback(
    (type) => {
      return loadingStates.has(type)
    },
    [loadingStates],
  )

  const isAnyLoading = loadingStates.size > 0

  const isAnyAuthLoading = [
    LOADING_TYPES.LOGIN,
    LOADING_TYPES.REGISTER,
    LOADING_TYPES.CHECK_AUTH,
    LOADING_TYPES.SEND_EMAIL,
    LOADING_TYPES.RESET_PASSWORD,
  ].some((type) => loadingStates.has(type))

  const isAnyUserLoading = [
    LOADING_TYPES.CHECK_USERNAME,
    LOADING_TYPES.CHANGE_USERNAME,
    LOADING_TYPES.CHECK_BIRTHDATE,
  ].some((type) => loadingStates.has(type))

  const value = {
    loadingStates: Array.from(loadingStates),
    setLoading,
    clearLoading,
    clearAllLoading,
    isLoading,
    isAnyLoading,
    isAnyAuthLoading,
    isAnyUserLoading,
    LOADING_TYPES,
  }

  return <LoadingContext.Provider value={value}>{children}</LoadingContext.Provider>
}
