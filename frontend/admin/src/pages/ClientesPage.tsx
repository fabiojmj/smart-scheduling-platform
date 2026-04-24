import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useAuth } from '@/hooks/useAuth'
import { useToast } from '@/hooks/useToast'
import { clientesApi, extractError } from '@/lib/api'
import { Modal } from '@/components/ui/Modal'
import type { Cliente } from '@/types'

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
const PHONE_REGEX = /^\+?[\d\s\-().]{8,20}$/

const initForm = () => ({ nome: '', telefone: '', email: '' })

export default function ClientesPage() {
  const { estabelecimentoId } = useAuth()
  const toast = useToast()
  const queryClient = useQueryClient()

  const [createOpen, setCreateOpen] = useState(false)
  const [search, setSearch]         = useState('')
  const [form, setForm]             = useState(initForm())
  const [errors, setErrors]         = useState<Partial<typeof form>>({})

  const { data: clientes = [], isLoading } = useQuery<Cliente[]>({
    queryKey: ['clientes', estabelecimentoId],
    queryFn: () => clientesApi.listar(estabelecimentoId!).then(r => r.data),
    enabled: !!estabelecimentoId,
  })

  const createMutation = useMutation({
    mutationFn: ({ nome, telefone, email }: typeof form) =>
      clientesApi.criar(nome, telefone, estabelecimentoId!, email || undefined),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clientes', estabelecimentoId] })
      toast.success('Cliente criado.')
      setCreateOpen(false)
      setForm(initForm())
    },
    onError: err => toast.error(extractError(err)),
  })

  function validate() {
    const errs: Partial<typeof form> = {}
    if (!form.nome.trim() || form.nome.trim().length < 2)
      errs.nome = 'Nome deve ter pelo menos 2 caracteres'
    if (!PHONE_REGEX.test(form.telefone.trim()))
      errs.telefone = 'Telefone inválido'
    if (form.email.trim() && !EMAIL_REGEX.test(form.email.trim()))
      errs.email = 'Email inválido'
    setErrors(errs)
    return Object.keys(errs).length === 0
  }

  function handleCreate() {
    if (!validate() || !estabelecimentoId) return
    createMutation.mutate({
      nome:     form.nome.trim(),
      telefone: form.telefone.trim(),
      email:    form.email.trim().toLowerCase(),
    })
  }

  const filtered = clientes.filter(c => {
    const q = search.toLowerCase()
    return (
      c.nome.toLowerCase().includes(q) ||
      c.telefone.includes(q) ||
      (c.email ?? '').toLowerCase().includes(q)
    )
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-xl font-semibold text-gray-900">Clientes</h2>
        <button
          className="btn-primary"
          onClick={() => { setCreateOpen(true); setErrors({}) }}
        >
          Novo cliente
        </button>
      </div>

      <div className="mb-4">
        <input
          type="search"
          className="input max-w-sm"
          placeholder="Buscar por nome, telefone ou email…"
          value={search}
          onChange={e => setSearch(e.target.value)}
          maxLength={100}
        />
      </div>

      {isLoading ? (
        <p className="text-sm text-gray-500">Carregando…</p>
      ) : filtered.length === 0 ? (
        <div className="card text-center py-12">
          <p className="text-gray-400 text-sm">
            {search ? 'Nenhum cliente encontrado.' : 'Nenhum cliente cadastrado.'}
          </p>
          {!search && (
            <button className="btn-primary mt-4" onClick={() => setCreateOpen(true)}>
              Adicionar cliente
            </button>
          )}
        </div>
      ) : (
        <div className="card overflow-hidden p-0">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Nome</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Telefone</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 hidden sm:table-cell">Email</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 hidden md:table-cell">Desde</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {filtered.map(c => (
                <tr key={c.id} className="hover:bg-gray-50">
                  <td className="px-5 py-3">
                    <div className="flex items-center gap-2.5">
                      <div className="w-7 h-7 rounded-full bg-gray-100 flex items-center justify-center text-gray-600 font-medium text-xs flex-shrink-0">
                        {c.nome.charAt(0).toUpperCase()}
                      </div>
                      <span className="font-medium text-gray-900">{c.nome}</span>
                    </div>
                  </td>
                  <td className="px-5 py-3 text-gray-600 tabular-nums">{c.telefone}</td>
                  <td className="px-5 py-3 text-gray-500 hidden sm:table-cell">{c.email ?? '—'}</td>
                  <td className="px-5 py-3 text-gray-400 text-xs hidden md:table-cell">
                    {new Date(c.criadoEm).toLocaleDateString('pt-BR')}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          <div className="px-5 py-2.5 border-t border-gray-100 text-xs text-gray-400">
            {filtered.length} cliente{filtered.length !== 1 ? 's' : ''}
          </div>
        </div>
      )}

      <Modal open={createOpen} onClose={() => setCreateOpen(false)} title="Novo cliente" size="sm">
        <div className="space-y-4">
          <div>
            <label htmlFor="cl-nome" className="block text-sm font-medium text-gray-700 mb-1">
              Nome *
            </label>
            <input
              id="cl-nome"
              type="text"
              className="input"
              value={form.nome}
              onChange={e => setForm(f => ({ ...f, nome: e.target.value }))}
              maxLength={100}
              placeholder="Nome completo"
            />
            {errors.nome && <p className="text-xs text-red-600 mt-1" role="alert">{errors.nome}</p>}
          </div>
          <div>
            <label htmlFor="cl-tel" className="block text-sm font-medium text-gray-700 mb-1">
              Telefone *
            </label>
            <input
              id="cl-tel"
              type="tel"
              className="input"
              value={form.telefone}
              onChange={e => setForm(f => ({ ...f, telefone: e.target.value }))}
              maxLength={20}
              placeholder="+55 11 99999-9999"
            />
            {errors.telefone && (
              <p className="text-xs text-red-600 mt-1" role="alert">{errors.telefone}</p>
            )}
          </div>
          <div>
            <label htmlFor="cl-email" className="block text-sm font-medium text-gray-700 mb-1">
              Email <span className="text-gray-400 font-normal">(opcional)</span>
            </label>
            <input
              id="cl-email"
              type="email"
              className="input"
              value={form.email}
              onChange={e => setForm(f => ({ ...f, email: e.target.value }))}
              maxLength={254}
            />
            {errors.email && <p className="text-xs text-red-600 mt-1" role="alert">{errors.email}</p>}
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <button className="btn-secondary" onClick={() => setCreateOpen(false)}>Cancelar</button>
            <button
              className="btn-primary"
              onClick={handleCreate}
              disabled={createMutation.isPending}
            >
              {createMutation.isPending ? 'Criando…' : 'Criar cliente'}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  )
}
