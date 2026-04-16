import { useState, FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import { api } from '@/lib/api'

export default function LoginPage() {
  const [email, setEmail]     = useState('')
  const [error, setError]     = useState('')
  const [loading, setLoading] = useState(false)
  const [sent, setSent]       = useState(false)
  const { login }             = useAuth()
  const navigate              = useNavigate()

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      await api.post('/auth/magic-link', { email })
      setSent(true)
    } catch {
      setError('Não foi possível enviar o link. Verifique o email.')
    } finally {
      setLoading(false)
    }
  }

  if (sent) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
        <div className="card w-full max-w-sm text-center">
          <p className="text-lg font-medium text-gray-900 mb-2">Verifique seu email</p>
          <p className="text-sm text-gray-500">Enviamos um link de acesso para <strong>{email}</strong>.</p>
        </div>
      </div>
    )
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <div className="card w-full max-w-sm">
        <h2 className="text-xl font-semibold text-gray-900 mb-2">Meus agendamentos</h2>
        <p className="text-sm text-gray-500 mb-6">Informe seu email para receber o link de acesso.</p>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
            <input type="email" className="input" value={email} onChange={e => setEmail(e.target.value)} required />
          </div>
          {error && <p className="text-sm text-red-600">{error}</p>}
          <button type="submit" className="btn-primary w-full" disabled={loading}>
            {loading ? 'Enviando...' : 'Enviar link de acesso'}
          </button>
        </form>
      </div>
    </div>
  )
}
