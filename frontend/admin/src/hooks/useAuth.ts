import { useState, useEffect } from 'react'

interface AuthState {
  isAuthenticated: boolean
  token: string | null
  login: (token: string) => void
  logout: () => void
}

export function useAuth(): AuthState {
  const [token, setToken] = useState<string | null>(
    () => localStorage.getItem('token')
  )

  const isAuthenticated = !!token

  const login = (newToken: string) => {
    localStorage.setItem('token', newToken)
    setToken(newToken)
  }

  const logout = () => {
    localStorage.removeItem('token')
    setToken(null)
  }

  return { isAuthenticated, token, login, logout }
}
