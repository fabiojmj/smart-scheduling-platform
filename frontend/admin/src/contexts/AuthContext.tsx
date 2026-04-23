import { createContext, useContext, useState, useEffect, ReactNode } from 'react'
import type { Session, User } from '@/types'
import { estabelecimentosApi } from '@/lib/api'

interface AuthContextValue {
  session: Session | null
  isAuthenticated: boolean
  isReady: boolean
  login: (token: string, user: User) => Promise<void>
  logout: () => void
  setEstabelecimentoId: (id: string) => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<Session | null>(null)
  const [isReady, setIsReady] = useState(false)

  useEffect(() => {
    const stored = localStorage.getItem('session')
    if (stored) {
      try {
        const parsed = JSON.parse(stored) as Session
        if (parsed.token) setSession(parsed)
        else localStorage.removeItem('session')
      } catch {
        localStorage.removeItem('session')
      }
    }
    setIsReady(true)
  }, [])

  const persist = (s: Session) => {
    setSession(s)
    localStorage.setItem('session', JSON.stringify(s))
  }

  const login = async (token: string, user: User) => {
    const draft: Session = { token, user, estabelecimentoId: null }
    persist(draft)

    try {
      const { data } = await estabelecimentosApi.meu()
      persist({ ...draft, estabelecimentoId: data.id })
    } catch {
      // user has no establishment yet
    }
  }

  const logout = () => {
    setSession(null)
    localStorage.removeItem('session')
  }

  const setEstabelecimentoId = (id: string) => {
    if (!session) return
    persist({ ...session, estabelecimentoId: id })
  }

  return (
    <AuthContext.Provider value={{ session, isAuthenticated: !!session, isReady, login, logout, setEstabelecimentoId }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuthContext() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuthContext must be used within AuthProvider')
  return ctx
}
