import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useAuth } from '@/hooks/useAuth'
import { useToast } from '@/hooks/useToast'
import { funcionariosApi, servicosApi, extractError } from '@/lib/api'
import { Modal } from '@/components/ui/Modal'
import type { Funcionario, Servico } from '@/types'

const DIAS_SEMANA = ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado']
const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

type ActiveModal = 'horarios' | 'servicos' | 'deactivate' | null

const initCreateForm = () => ({ nome: '', email: '' })
const initHorasForm  = () => ({ diaSemana: '1', horaInicio: '08:00', horaFim: '18:00' })

export default function EmployeesPage() {
  const { estabelecimentoId } = useAuth()
  const toast = useToast()
  const queryClient = useQueryClient()

  const [createOpen, setCreateOpen]       = useState(false)
  const [activeModal, setActiveModal]     = useState<ActiveModal>(null)
  const [target, setTarget]               = useState<Funcionario | null>(null)
  const [createForm, setCreateForm]       = useState(initCreateForm())
  const [createErrors, setCreateErrors]   = useState<Partial<typeof createForm>>({})
  const [horasForm, setHorasForm]         = useState(initHorasForm())
  const [horasError, setHorasError]       = useState('')
  const [servicoId, setServicoId]         = useState('')

  const invalidate = () =>
    queryClient.invalidateQueries({ queryKey: ['funcionarios', estabelecimentoId] })

  // ─── Queries ──────────────────────────────────────────────────────────────

  const { data: funcionarios = [], isLoading } = useQuery<Funcionario[]>({
    queryKey: ['funcionarios', estabelecimentoId],
    queryFn: () => funcionariosApi.listar(estabelecimentoId!).then(r => r.data),
    enabled: !!estabelecimentoId,
  })

  const { data: servicos = [] } = useQuery<Servico[]>({
    queryKey: ['servicos', estabelecimentoId],
    queryFn: () => servicosApi.listar(estabelecimentoId!).then(r => r.data),
    enabled: !!estabelecimentoId && activeModal === 'servicos',
  })

  // ─── Mutations ────────────────────────────────────────────────────────────

  const createMutation = useMutation({
    mutationFn: ({ nome, email }: typeof createForm) =>
      funcionariosApi.criar(nome, email, estabelecimentoId!),
    onSuccess: () => {
      invalidate()
      toast.success('Funcionário criado com sucesso.')
      setCreateOpen(false)
      setCreateForm(initCreateForm())
    },
    onError: err => toast.error(extractError(err)),
  })

  const deactivateMutation = useMutation({
    mutationFn: (id: string) => funcionariosApi.desativar(id),
    onSuccess: () => {
      invalidate()
      toast.success('Funcionário desativado.')
      setActiveModal(null)
      setTarget(null)
    },
    onError: err => toast.error(extractError(err)),
  })

  const addHorarioMutation = useMutation({
    mutationFn: ({
      id, diaSemana, horaInicio, horaFim,
    }: { id: string; diaSemana: number; horaInicio: string; horaFim: string }) =>
      funcionariosApi.adicionarHorario(id, diaSemana, horaInicio, horaFim),
    onSuccess: () => {
      invalidate()
      toast.success('Horário adicionado.')
      setHorasForm(initHorasForm())
    },
    onError: err => toast.error(extractError(err)),
  })

  const atribuirServicoMutation = useMutation({
    mutationFn: ({ id, sId }: { id: string; sId: string }) =>
      funcionariosApi.atribuirServico(id, sId),
    onSuccess: () => {
      invalidate()
      toast.success('Serviço atribuído.')
      setServicoId('')
    },
    onError: err => toast.error(extractError(err)),
  })

  // ─── Handlers ────────────────────────────────────────────────────────────

  function validateCreate() {
    const errs: Partial<typeof createForm> = {}
    if (!createForm.nome.trim() || createForm.nome.trim().length < 2)
      errs.nome = 'Nome deve ter pelo menos 2 caracteres'
    if (!EMAIL_REGEX.test(createForm.email.trim()))
      errs.email = 'Email inválido'
    setCreateErrors(errs)
    return Object.keys(errs).length === 0
  }

  function handleCreate() {
    if (!validateCreate() || !estabelecimentoId) return
    createMutation.mutate({
      nome:  createForm.nome.trim(),
      email: createForm.email.trim().toLowerCase(),
    })
  }

  function handleAddHorario() {
    if (!target) return
    if (horasForm.horaInicio >= horasForm.horaFim) {
      setHorasError('Hora de início deve ser anterior à hora de fim.')
      return
    }
    setHorasError('')
    addHorarioMutation.mutate({
      id:        target.id,
      diaSemana: parseInt(horasForm.diaSemana, 10),
      horaInicio: horasForm.horaInicio,
      horaFim:    horasForm.horaFim,
    })
  }

  function openModal(f: Funcionario, modal: ActiveModal) {
    setTarget(f)
    setActiveModal(modal)
    if (modal === 'horarios') { setHorasForm(initHorasForm()); setHorasError('') }
    if (modal === 'servicos') setServicoId('')
  }

  function closeModal() { setTarget(null); setActiveModal(null) }

  // ─── Render ───────────────────────────────────────────────────────────────

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-xl font-semibold text-gray-900">Funcionários</h2>
        <button
          className="btn-primary"
          onClick={() => { setCreateOpen(true); setCreateErrors({}) }}
        >
          Novo funcionário
        </button>
      </div>

      {isLoading ? (
        <p className="text-sm text-gray-500">Carregando…</p>
      ) : funcionarios.length === 0 ? (
        <div className="card text-center py-12">
          <p className="text-gray-400 text-sm">Nenhum funcionário cadastrado.</p>
          <button className="btn-primary mt-4" onClick={() => setCreateOpen(true)}>
            Adicionar funcionário
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {funcionarios.map(f => (
            <div key={f.id} className="card flex flex-col gap-3">
              <div className="flex items-start gap-3">
                <div className="w-10 h-10 rounded-full bg-primary-50 flex items-center justify-center text-primary-800 font-semibold text-sm flex-shrink-0">
                  {f.nome.charAt(0).toUpperCase()}
                </div>
                <div className="min-w-0 flex-1">
                  <p className="font-medium text-gray-900 text-sm truncate">{f.nome}</p>
                  <p className="text-xs text-gray-500 truncate">{f.email}</p>
                </div>
                <span
                  className={`inline-flex shrink-0 rounded-full px-2 py-0.5 text-xs font-medium ${
                    f.ativo ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-500'
                  }`}
                >
                  {f.ativo ? 'Ativo' : 'Inativo'}
                </span>
              </div>

              {f.horarios && f.horarios.length > 0 && (
                <div className="text-xs text-gray-500 space-y-0.5 border-t border-gray-100 pt-2">
                  {f.horarios.map((h, i) => (
                    <div key={i} className="flex justify-between">
                      <span>{DIAS_SEMANA[Number(h.diaSemana)] ?? `Dia ${h.diaSemana}`}</span>
                      <span>{h.horaInicio} – {h.horaFim}</span>
                    </div>
                  ))}
                </div>
              )}

              <div className="flex gap-2 pt-1 border-t border-gray-100">
                <button
                  className="btn-secondary flex-1 text-xs py-1.5"
                  onClick={() => openModal(f, 'horarios')}
                >
                  Horários
                </button>
                <button
                  className="btn-secondary flex-1 text-xs py-1.5"
                  onClick={() => openModal(f, 'servicos')}
                >
                  Serviços
                </button>
                {f.ativo && (
                  <button
                    className="inline-flex items-center justify-center rounded-lg border border-red-200 px-2.5 py-1.5 text-xs font-medium text-red-600 hover:bg-red-50 transition-colors"
                    onClick={() => openModal(f, 'deactivate')}
                  >
                    Desativar
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {/* ── Criar funcionário ─────────────────────────────────────── */}
      <Modal open={createOpen} onClose={() => setCreateOpen(false)} title="Novo funcionário" size="sm">
        <div className="space-y-4">
          <div>
            <label htmlFor="fn-nome" className="block text-sm font-medium text-gray-700 mb-1">
              Nome *
            </label>
            <input
              id="fn-nome"
              type="text"
              className="input"
              value={createForm.nome}
              onChange={e => setCreateForm(f => ({ ...f, nome: e.target.value }))}
              maxLength={100}
              placeholder="Nome completo"
            />
            {createErrors.nome && (
              <p className="text-xs text-red-600 mt-1" role="alert">{createErrors.nome}</p>
            )}
          </div>
          <div>
            <label htmlFor="fn-email" className="block text-sm font-medium text-gray-700 mb-1">
              Email *
            </label>
            <input
              id="fn-email"
              type="email"
              className="input"
              value={createForm.email}
              onChange={e => setCreateForm(f => ({ ...f, email: e.target.value }))}
              maxLength={254}
              placeholder="email@exemplo.com"
            />
            {createErrors.email && (
              <p className="text-xs text-red-600 mt-1" role="alert">{createErrors.email}</p>
            )}
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <button className="btn-secondary" onClick={() => setCreateOpen(false)}>Cancelar</button>
            <button
              className="btn-primary"
              onClick={handleCreate}
              disabled={createMutation.isPending}
            >
              {createMutation.isPending ? 'Criando…' : 'Criar'}
            </button>
          </div>
        </div>
      </Modal>

      {/* ── Horários ──────────────────────────────────────────────── */}
      <Modal
        open={activeModal === 'horarios'}
        onClose={closeModal}
        title={`Horários — ${target?.nome ?? ''}`}
        size="md"
      >
        <div className="space-y-4">
          <div className="grid grid-cols-3 gap-3">
            <div>
              <label className="block text-xs font-medium text-gray-700 mb-1">Dia</label>
              <select
                className="input"
                value={horasForm.diaSemana}
                onChange={e => setHorasForm(f => ({ ...f, diaSemana: e.target.value }))}
              >
                {DIAS_SEMANA.map((d, i) => (
                  <option key={i} value={i}>{d}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-700 mb-1">Início</label>
              <input
                type="time"
                className="input"
                value={horasForm.horaInicio}
                onChange={e => setHorasForm(f => ({ ...f, horaInicio: e.target.value }))}
              />
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-700 mb-1">Fim</label>
              <input
                type="time"
                className="input"
                value={horasForm.horaFim}
                onChange={e => setHorasForm(f => ({ ...f, horaFim: e.target.value }))}
              />
            </div>
          </div>
          {horasError && <p className="text-xs text-red-600" role="alert">{horasError}</p>}
          <button
            className="btn-primary"
            onClick={handleAddHorario}
            disabled={addHorarioMutation.isPending}
          >
            {addHorarioMutation.isPending ? 'Adicionando…' : 'Adicionar horário'}
          </button>

          {target?.horarios && target.horarios.length > 0 && (
            <div>
              <p className="text-xs font-medium text-gray-500 mb-2">Horários cadastrados</p>
              <div className="space-y-1">
                {target.horarios.map((h, i) => (
                  <div
                    key={i}
                    className="flex items-center justify-between text-sm bg-gray-50 rounded-lg px-3 py-2"
                  >
                    <span className="font-medium text-gray-700">
                      {DIAS_SEMANA[Number(h.diaSemana)] ?? `Dia ${h.diaSemana}`}
                    </span>
                    <span className="text-gray-500">{h.horaInicio} – {h.horaFim}</span>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </Modal>

      {/* ── Serviços ──────────────────────────────────────────────── */}
      <Modal
        open={activeModal === 'servicos'}
        onClose={closeModal}
        title={`Atribuir serviço — ${target?.nome ?? ''}`}
        size="sm"
      >
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Serviço</label>
            <select
              className="input"
              value={servicoId}
              onChange={e => setServicoId(e.target.value)}
            >
              <option value="">Selecione um serviço</option>
              {servicos.filter(s => s.ativo).map(s => (
                <option key={s.id} value={s.id}>
                  {s.nome} — {s.duracaoMinutos}min
                </option>
              ))}
            </select>
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <button className="btn-secondary" onClick={closeModal}>Cancelar</button>
            <button
              className="btn-primary"
              onClick={() => target && servicoId && atribuirServicoMutation.mutate({ id: target.id, sId: servicoId })}
              disabled={!servicoId || atribuirServicoMutation.isPending}
            >
              {atribuirServicoMutation.isPending ? 'Atribuindo…' : 'Atribuir'}
            </button>
          </div>
        </div>
      </Modal>

      {/* ── Desativar ─────────────────────────────────────────────── */}
      <Modal
        open={activeModal === 'deactivate'}
        onClose={closeModal}
        title="Desativar funcionário"
        size="sm"
      >
        <div className="space-y-4">
          <p className="text-sm text-gray-700">
            Desativar <span className="font-medium">{target?.nome}</span>? O funcionário não
            aparecerá mais para novos agendamentos.
          </p>
          <div className="flex justify-end gap-2">
            <button className="btn-secondary" onClick={closeModal}>Cancelar</button>
            <button
              className="inline-flex items-center justify-center rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 disabled:opacity-50 transition-colors"
              onClick={() => target && deactivateMutation.mutate(target.id)}
              disabled={deactivateMutation.isPending}
            >
              {deactivateMutation.isPending ? 'Desativando…' : 'Desativar'}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  )
}
