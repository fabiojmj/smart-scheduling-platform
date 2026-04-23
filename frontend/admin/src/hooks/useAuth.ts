import { useAuthContext } from '@/contexts/AuthContext'

export function useAuth() {
  const { session, isAuthenticated, isReady, login, logout, setEstabelecimentoId } = useAuthContext()
  return {
    isAuthenticated,
    isReady,
    user: session?.user ?? null,
    token: session?.token ?? null,
    estabelecimentoId: session?.estabelecimentoId ?? null,
    login,
    logout,
    setEstabelecimentoId,
  }
}
