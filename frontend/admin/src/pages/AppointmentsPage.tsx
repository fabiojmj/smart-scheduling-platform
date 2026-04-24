import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useAuth } from '@/hooks/useAuth'
import { useToast } from '@/hooks/useToast'
import { agendamentosApi, funcionariosApi, servicosApi, clientesApi, extractError } from '@/lib/api'
import { Modal } from '@/components/ui/Modal'
import type { Agendamento, Funcionario, Servico, Cliente } from '@/types'

const STATUS_MAP: Record<string, { label: string; className: string }> = {
  Pending:   { label: 'Pendente',   className: 'bg-yellow-100 text-yellow-800' },
  Confirmed: { label: 'Confirmado', className: 'bg-green-100 text-green-800' },
  Completed: { label: 'Concluído',  className: 'bg-blue-100 text-blue-800' },
  Cancelled: { label: 'Cancelado',  className: 'bg-red-100 text-red-800' },
  NoShow:    { label: 'Não veio',   className: 'bg-gray-100 text-gray-600' },
}

function StatusBadge({ status }: { status: string }) {
  const s = STATUS_MAP[status] ?? { label: status, className: 'bg-gray-100 text-gray-600' }
  return (
    <span className={`inline-flex rounded-full px-2 py-0.5 text-xs font-medium ${s.className}`}>
      {s.label}
    </span>
  )
}

const initCreateForm = () => ({
  clienteId: '', funcionarioId: '', servicoId: '', start: '', observacoes: '',
})

export default function AppointmentsPage() {
  const { estabelecimentoId } = useAuth()
  const toast = useToast()
  const queryClient = useQueryClient()

  const today = new Date().toISOString().split('T')[0]
  const [selectedDate, setSelectedDate] = useState(today)

  const [cancelTarget, setCancelTarget]   = useState<Agendamento | null>(null)
  const [cancelMotivo, setCancelMotivo]   = useState('')
  const [cancelError, setCancelError]     = useState('')

  const [createOpen, setCreateOpen]       = useState(false)
  const [createForm, setCreateForm]       = useState(initCreateForm())
  const [createErrors, setCreateErrors]   = useState<Partial<typeof createForm>>({})

  const invalidate = () =>
    queryClient.invalidateQueries({ queryKey: ['agendamentos', estabelecimentoId, selectedDate] })

  // ─── Queries ──────────────────────────────────────────────────────────────

  const { data: agendamentos = [], isLoading } = useQuery<Agendamento[]>({
    queryKey: ['agendamentos', estabelecimentoId, selectedDate],
    queryFn: () =>
      agendamentosApi.listarPorEstabelecimento(estabelecimentoId!, selectedDate).then(r => r.data),
    enabled: !!estabelecimentoId,
  })

  const { data: clientes = [] } = useQuery<Cliente[]>({
    queryKey: ['clientes', estabelecimentoId],
    queryFn: () => clientesApi.listar(estabelecimentoId!).then(r => r.data),
    enabled: !!estabelecimentoId && createOpen,
  })

  const { data: funcionarios = [] } = useQuery<Funcionario[]>({
    queryKey: ['funcionarios', estabelecimentoId],
    queryFn: () => funcionariosApi.listar(estabelecimentoId!).then(r => r.data),
    enabled: !!estabelecimentoId && createOpen,
  })

  const { data: servicos = [] } = useQuery<Servico[]>({
    queryKey: ['servicos', estabelecimentoId],
    queryFn: () => servicosApi.listar(estabelecimentoId!).then(r => r.data),
    enabled: !!estabelecimentoId && createOpen,
  })

  // ─── Mutations ────────────────────────────────────────────────────────────

  const confirmarMutation = useMutation({
    mutationFn: (id: string) => agendamentosApi.confirmar(id),
    onSuccess: () => { invalidate(); toast.success('Agendamento confirmado.') },
    onError:   err => toast.error(extractError(err)),
  })

  const concluirMutation = useMutation({
    mutationFn: (id: string) => agendamentosApi.concluir(id),
    onSuccess: () => { invalidate(); toast.success('Agendamento concluído.') },
    onError:   err => toast.error(extractError(err)),
  })

  const naoCompareceuMutation = useMutation({
    mutationFn: (id: string) => agendamentosApi.naoCompareceu(id),
    onSuccess: () => { invalidate(); toast.success('Marcado como não compareceu.') },
    onError:   err => toast.error(extractError(err)),
  })

  const cancelarMutation = useMutation({
    mutationFn: ({ id, motivo }: { id: string; motivo: string }) =>
      agendamentosApi.cancelar(id, motivo),
    onSuccess: () => {
      invalidate()
      toast.success('Agendamento cancelado.')
      setCancelTarget(null)
      setCancelMotivo('')
    },
    onError: err => toast.error(extractError(err)),
  })

  const criarMutation = useMutation({
    mutationFn: (f: typeof createForm) =>
      agendamentosApi.criar(f.clienteId, f.funcionarioId, f.servicoId, f.start, f.observacoes || undefined),
    onSuccess: () => {
      invalidate()
      toast.success('Agendamento criado.')
      setCreateOpen(false)
      setCreateForm(initCreateForm())
    },
    onError: err => toast.error(extractError(err)),
  })

  // ─── Handlers ────────────────────────────────────────────────────────────

  function handleCancelar() {
    if (!cancelTarget) return
    if (!cancelMotivo.trim()) { setCancelError('Motivo é obrigatório.'); return }
    setCancelError('')
    cancelarMutation.mutate({ id: cancelTarget.id, motivo: cancelMotivo.trim() })
  }

  function validateCreate() {
    const errs: Partial<typeof createForm> = {}
    if (!createForm.clienteId)    errs.clienteId    = 'Selecione um cliente'
    if (!createForm.funcionarioId) errs.funcionarioId = 'Selecione um funcionário'
    if (!createForm.servicoId)    errs.servicoId    = 'Selecione um serviço'
    if (!createForm.start)        errs.start        = 'Data e horário são obrigatórios'
    else if (new Date(createForm.start) < new Date()) errs.start = 'Data deve ser futura'
    setCreateErrors(errs)
    return Object.keys(errs).length === 0
  }

  function handleCreate() {
    if (!validateCreate()) return
    criarMutation.mutate(createForm)
  }

  const sorted = [...agendamentos].sort(
    (a, b) => new Date(a.inicioEm).getTime() - new Date(b.inicioEm).getTime(),
  )

  // ─── Render ───────────────────────────────────────────────────────────────

  return (
    <div>
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 mb-6">
        <h2 className="text-xl font-semibold text-gray-900">Agendamentos</h2>
        <div className="flex items-center gap-3">
          <input
            type="date"
            className="input text-sm py-1.5"
            value={selectedDate}
            onChange={e => setSelectedDate(e.target.value)}
          />
          <button
            className="btn-primary whitespace-nowrap"
            onClick={() => { setCreateOpen(true); setCreateErrors({}) }}
          >
            Novo agendamento
          </button>
        </div>
      </div>

      {isLoading ? (
        <p className="text-sm text-gray-500">Carregando…</p>
      ) : (
        <div className="card overflow-hidden p-0">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Horário</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Cliente</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 hidden sm:table-cell">Serviço</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500 hidden md:table-cell">Funcionário</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Status</th>
                <th className="px-5 py-3 text-right text-xs font-medium text-gray-500">Ações</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {sorted.length === 0 ? (
                <tr>
                  <td colSpan={6} className="px-5 py-10 text-center text-gray-400 text-sm">
                    Nenhum agendamento para esta data.
                  </td>
                </tr>
              ) : sorted.map(a => (
                <tr key={a.id} className="hover:bg-gray-50">
                  <td className="px-5 py-3 font-medium text-gray-900 tabular-nums whitespace-nowrap">
                    {new Date(a.inicioEm).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}
                    <span className="text-gray-400 font-normal">
                      {' '}–{' '}
                      {new Date(a.fimEm).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}
                    </span>
                  </td>
                  <td className="px-5 py-3 text-gray-700">{a.nomeCliente}</td>
                  <td className="px-5 py-3 text-gray-600 hidden sm:table-cell">{a.nomeServico}</td>
                  <td className="px-5 py-3 text-gray-600 hidden md:table-cell">{a.nomeFuncionario}</td>
                  <td className="px-5 py-3"><StatusBadge status={a.status} /></td>
                  <td className="px-5 py-3">
                    <div className="flex items-center justify-end gap-2 text-xs font-medium">
                      {a.status === 'Pending' && (
                        <button
                          className="text-green-600 hover:text-green-800"
                          onClick={() => confirmarMutation.mutate(a.id)}
                          disabled={confirmarMutation.isPending}
                        >
                          Confirmar
                        </button>
                      )}
                      {a.status === 'Confirmed' && (
                        <>
                          <button
                            className="text-blue-600 hover:text-blue-800"
                            onClick={() => concluirMutation.mutate(a.id)}
                            disabled={concluirMutation.isPending}
                          >
                            Concluir
                          </button>
                          <span className="text-gray-300">·</span>
                          <button
                            className="text-gray-600 hover:text-gray-900"
                            onClick={() => naoCompareceuMutation.mutate(a.id)}
                            disabled={naoCompareceuMutation.isPending}
                          >
                            Não veio
                          </button>
                        </>
                      )}
                      {(a.status === 'Pending' || a.status === 'Confirmed') && (
                        <>
                          <span className="text-gray-300">·</span>
                          <button
                            className="text-red-600 hover:text-red-800"
                            onClick={() => { setCancelTarget(a); setCancelMotivo(''); setCancelError('') }}
                          >
                            Cancelar
                          </button>
                        </>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* ── Cancelar agendamento ──────────────────────────────────── */}
      <Modal
        open={!!cancelTarget}
        onClose={() => setCancelTarget(null)}
        title="Cancelar agendamento"
        size="sm"
      >
        <div className="space-y-4">
          <p className="text-sm text-gray-700">
            Cancelar o agendamento de{' '}
            <span className="font-medium">{cancelTarget?.nomeCliente}</span>
            {cancelTarget && (
              <> — {cancelTarget.nomeServico} às{' '}
                {new Date(cancelTarget.inicioEm).toLocaleTimeString('pt-BR', {
                  hour: '2-digit', minute: '2-digit',
                })}
              </>
            )}
          </p>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Motivo *</label>
            <textarea
              className="input resize-none"
              rows={3}
              value={cancelMotivo}
              onChange={e => setCancelMotivo(e.target.value)}
              placeholder="Informe o motivo do cancelamento"
              maxLength={500}
            />
            {cancelError && <p className="text-xs text-red-600 mt-1" role="alert">{cancelError}</p>}
          </div>
          <div className="flex justify-end gap-2">
            <button className="btn-secondary" onClick={() => setCancelTarget(null)}>Voltar</button>
            <button
              className="inline-flex items-center justify-center rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 disabled:opacity-50 transition-colors"
              onClick={handleCancelar}
              disabled={cancelarMutation.isPending}
            >
              {cancelarMutation.isPending ? 'Cancelando…' : 'Cancelar agendamento'}
            </button>
          </div>
        </div>
      </Modal>

      {/* ── Novo agendamento ──────────────────────────────────────── */}
      <Modal open={createOpen} onClose={() => setCreateOpen(false)} title="Novo agendamento" size="md">
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Cliente *</label>
            <select
              className="input"
              value={createForm.clienteId}
              onChange={e => setCreateForm(f => ({ ...f, clienteId: e.target.value }))}
            >
              <option value="">Selecione</option>
              {clientes.map(c => (
                <option key={c.id} value={c.id}>{c.nome} — {c.telefone}</option>
              ))}
            </select>
            {createErrors.clienteId && (
              <p className="text-xs text-red-600 mt-1" role="alert">{createErrors.clienteId}</p>
            )}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Funcionário *</label>
            <select
              className="input"
              value={createForm.funcionarioId}
              onChange={e => setCreateForm(f => ({ ...f, funcionarioId: e.target.value }))}
            >
              <option value="">Selecione</option>
              {funcionarios.filter(f => f.ativo).map(f => (
                <option key={f.id} value={f.id}>{f.nome}</option>
              ))}
            </select>
            {createErrors.funcionarioId && (
              <p className="text-xs text-red-600 mt-1" role="alert">{createErrors.funcionarioId}</p>
            )}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Serviço *</label>
            <select
              className="input"
              value={createForm.servicoId}
              onChange={e => setCreateForm(f => ({ ...f, servicoId: e.target.value }))}
            >
              <option value="">Selecione</option>
              {servicos.filter(s => s.ativo).map(s => (
                <option key={s.id} value={s.id}>
                  {s.nome} — {s.duracaoMinutos}min —{' '}
                  {s.preco.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                </option>
              ))}
            </select>
            {createErrors.servicoId && (
              <p className="text-xs text-red-600 mt-1" role="alert">{createErrors.servicoId}</p>
            )}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Data e horário *</label>
            <input
              type="datetime-local"
              className="input"
              value={createForm.start}
              onChange={e => setCreateForm(f => ({ ...f, start: e.target.value }))}
            />
            {createErrors.start && (
              <p className="text-xs text-red-600 mt-1" role="alert">{createErrors.start}</p>
            )}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Observações</label>
            <textarea
              className="input resize-none"
              rows={2}
              value={createForm.observacoes}
              onChange={e => setCreateForm(f => ({ ...f, observacoes: e.target.value }))}
              maxLength={500}
            />
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <button className="btn-secondary" onClick={() => setCreateOpen(false)}>Cancelar</button>
            <button
              className="btn-primary"
              onClick={handleCreate}
              disabled={criarMutation.isPending}
            >
              {criarMutation.isPending ? 'Criando…' : 'Criar agendamento'}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  )
}
