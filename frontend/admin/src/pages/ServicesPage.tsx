import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useAuth } from '@/hooks/useAuth'
import { useToast } from '@/hooks/useToast'
import { servicosApi, extractError } from '@/lib/api'
import { Modal } from '@/components/ui/Modal'
import type { Servico } from '@/types'

type EditTarget = { servico: Servico; field: 'preco' | 'duracao' } | null

const initCreateForm = () => ({ nome: '', descricao: '', duracaoMinutos: '', preco: '' })

export default function ServicesPage() {
  const { estabelecimentoId } = useAuth()
  const toast = useToast()
  const queryClient = useQueryClient()

  const [createOpen, setCreateOpen]   = useState(false)
  const [editTarget, setEditTarget]   = useState<EditTarget>(null)
  const [confirmDeact, setConfirmDeact] = useState<Servico | null>(null)
  const [createForm, setCreateForm]   = useState(initCreateForm())
  const [createErrors, setCreateErrors] = useState<Partial<Record<keyof typeof createForm, string>>>({})
  const [editValue, setEditValue]     = useState('')

  const invalidate = () =>
    queryClient.invalidateQueries({ queryKey: ['servicos', estabelecimentoId] })

  // ─── Queries ──────────────────────────────────────────────────────────────

  const { data: servicos = [], isLoading } = useQuery<Servico[]>({
    queryKey: ['servicos', estabelecimentoId],
    queryFn: () => servicosApi.listar(estabelecimentoId!).then(r => r.data),
    enabled: !!estabelecimentoId,
  })

  // ─── Mutations ────────────────────────────────────────────────────────────

  const createMutation = useMutation({
    mutationFn: (p: { nome: string; descricao: string; duracaoMinutos: number; preco: number }) =>
      servicosApi.criar(p.nome, p.duracaoMinutos, p.preco, estabelecimentoId!, p.descricao || undefined),
    onSuccess: () => {
      invalidate()
      toast.success('Serviço criado.')
      setCreateOpen(false)
      setCreateForm(initCreateForm())
    },
    onError: err => toast.error(extractError(err)),
  })

  const updatePriceMutation = useMutation({
    mutationFn: ({ id, preco }: { id: string; preco: number }) =>
      servicosApi.atualizarPreco(id, preco),
    onSuccess: () => { invalidate(); toast.success('Preço atualizado.'); setEditTarget(null) },
    onError:   err => toast.error(extractError(err)),
  })

  const updateDuracaoMutation = useMutation({
    mutationFn: ({ id, duracao }: { id: string; duracao: number }) =>
      servicosApi.atualizarDuracao(id, duracao),
    onSuccess: () => { invalidate(); toast.success('Duração atualizada.'); setEditTarget(null) },
    onError:   err => toast.error(extractError(err)),
  })

  const deactivateMutation = useMutation({
    mutationFn: (id: string) => servicosApi.desativar(id),
    onSuccess: () => { invalidate(); toast.success('Serviço desativado.'); setConfirmDeact(null) },
    onError:   err => toast.error(extractError(err)),
  })

  // ─── Handlers ────────────────────────────────────────────────────────────

  function validateCreate() {
    const errs: Partial<Record<keyof typeof createForm, string>> = {}
    if (!createForm.nome.trim()) errs.nome = 'Nome é obrigatório'
    const dur = parseFloat(createForm.duracaoMinutos)
    if (!createForm.duracaoMinutos || isNaN(dur) || dur < 1)
      errs.duracaoMinutos = 'Duração inválida (mín. 1 min)'
    const preco = parseFloat(createForm.preco)
    if (createForm.preco === '' || isNaN(preco) || preco < 0)
      errs.preco = 'Preço inválido'
    setCreateErrors(errs)
    return Object.keys(errs).length === 0
  }

  function handleCreate() {
    if (!validateCreate() || !estabelecimentoId) return
    createMutation.mutate({
      nome:           createForm.nome.trim(),
      descricao:      createForm.descricao.trim(),
      duracaoMinutos: Math.max(1, Math.round(parseFloat(createForm.duracaoMinutos))),
      preco:          parseFloat(parseFloat(createForm.preco).toFixed(2)),
    })
  }

  function handleSaveEdit() {
    if (!editTarget) return
    const { servico, field } = editTarget
    const val = parseFloat(editValue)
    if (isNaN(val) || val < 0 || (field === 'duracao' && val < 1)) {
      toast.error(field === 'preco' ? 'Preço inválido' : 'Duração inválida')
      return
    }
    if (field === 'preco')
      updatePriceMutation.mutate({ id: servico.id, preco: parseFloat(val.toFixed(2)) })
    else
      updateDuracaoMutation.mutate({ id: servico.id, duracao: Math.round(val) })
  }

  function openEdit(servico: Servico, field: 'preco' | 'duracao') {
    setEditTarget({ servico, field })
    setEditValue(String(field === 'preco' ? servico.preco : servico.duracaoMinutos))
  }

  const isSaving = updatePriceMutation.isPending || updateDuracaoMutation.isPending

  // ─── Render ───────────────────────────────────────────────────────────────

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-xl font-semibold text-gray-900">Serviços</h2>
        <button
          className="btn-primary"
          onClick={() => { setCreateOpen(true); setCreateErrors({}) }}
        >
          Novo serviço
        </button>
      </div>

      {isLoading ? (
        <p className="text-sm text-gray-500">Carregando…</p>
      ) : servicos.length === 0 ? (
        <div className="card text-center py-12">
          <p className="text-gray-400 text-sm">Nenhum serviço cadastrado.</p>
          <button className="btn-primary mt-4" onClick={() => setCreateOpen(true)}>
            Adicionar serviço
          </button>
        </div>
      ) : (
        <div className="card overflow-hidden p-0">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Nome</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Duração</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Preço</th>
                <th className="px-5 py-3 text-left text-xs font-medium text-gray-500">Status</th>
                <th className="px-5 py-3 text-right text-xs font-medium text-gray-500">Ações</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {servicos.map(s => (
                <tr key={s.id} className="hover:bg-gray-50">
                  <td className="px-5 py-3">
                    <p className="font-medium text-gray-900">{s.nome}</p>
                    {s.descricao && (
                      <p className="text-xs text-gray-400 mt-0.5 truncate max-w-xs">{s.descricao}</p>
                    )}
                  </td>
                  <td className="px-5 py-3 text-gray-600 tabular-nums">{s.duracaoMinutos} min</td>
                  <td className="px-5 py-3 text-gray-600 tabular-nums">
                    {s.preco.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                  </td>
                  <td className="px-5 py-3">
                    <span
                      className={`inline-flex rounded-full px-2 py-0.5 text-xs font-medium ${
                        s.ativo ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-500'
                      }`}
                    >
                      {s.ativo ? 'Ativo' : 'Inativo'}
                    </span>
                  </td>
                  <td className="px-5 py-3">
                    <div className="flex items-center justify-end gap-2 text-xs font-medium">
                      <button
                        className="text-blue-600 hover:text-blue-800"
                        onClick={() => openEdit(s, 'preco')}
                      >
                        Preço
                      </button>
                      <span className="text-gray-300">·</span>
                      <button
                        className="text-blue-600 hover:text-blue-800"
                        onClick={() => openEdit(s, 'duracao')}
                      >
                        Duração
                      </button>
                      {s.ativo && (
                        <>
                          <span className="text-gray-300">·</span>
                          <button
                            className="text-red-600 hover:text-red-800"
                            onClick={() => setConfirmDeact(s)}
                          >
                            Desativar
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

      {/* ── Criar serviço ─────────────────────────────────────────── */}
      <Modal open={createOpen} onClose={() => setCreateOpen(false)} title="Novo serviço" size="md">
        <div className="space-y-4">
          <div>
            <label htmlFor="sv-nome" className="block text-sm font-medium text-gray-700 mb-1">
              Nome *
            </label>
            <input
              id="sv-nome"
              type="text"
              className="input"
              value={createForm.nome}
              onChange={e => setCreateForm(f => ({ ...f, nome: e.target.value }))}
              maxLength={100}
              placeholder="Ex: Corte de cabelo"
            />
            {createErrors.nome && (
              <p className="text-xs text-red-600 mt-1" role="alert">{createErrors.nome}</p>
            )}
          </div>
          <div>
            <label htmlFor="sv-desc" className="block text-sm font-medium text-gray-700 mb-1">
              Descrição
            </label>
            <textarea
              id="sv-desc"
              className="input resize-none"
              rows={2}
              value={createForm.descricao}
              onChange={e => setCreateForm(f => ({ ...f, descricao: e.target.value }))}
              maxLength={500}
            />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label htmlFor="sv-dur" className="block text-sm font-medium text-gray-700 mb-1">
                Duração (min) *
              </label>
              <input
                id="sv-dur"
                type="number"
                className="input"
                min={1}
                max={480}
                value={createForm.duracaoMinutos}
                onChange={e => setCreateForm(f => ({ ...f, duracaoMinutos: e.target.value }))}
                placeholder="30"
              />
              {createErrors.duracaoMinutos && (
                <p className="text-xs text-red-600 mt-1" role="alert">{createErrors.duracaoMinutos}</p>
              )}
            </div>
            <div>
              <label htmlFor="sv-preco" className="block text-sm font-medium text-gray-700 mb-1">
                Preço (R$) *
              </label>
              <input
                id="sv-preco"
                type="number"
                className="input"
                min={0}
                step={0.01}
                value={createForm.preco}
                onChange={e => setCreateForm(f => ({ ...f, preco: e.target.value }))}
                placeholder="50.00"
              />
              {createErrors.preco && (
                <p className="text-xs text-red-600 mt-1" role="alert">{createErrors.preco}</p>
              )}
            </div>
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <button className="btn-secondary" onClick={() => setCreateOpen(false)}>Cancelar</button>
            <button
              className="btn-primary"
              onClick={handleCreate}
              disabled={createMutation.isPending}
            >
              {createMutation.isPending ? 'Criando…' : 'Criar serviço'}
            </button>
          </div>
        </div>
      </Modal>

      {/* ── Editar preço / duração ────────────────────────────────── */}
      <Modal
        open={!!editTarget}
        onClose={() => setEditTarget(null)}
        title={
          editTarget?.field === 'preco'
            ? `Alterar preço — ${editTarget.servico.nome}`
            : `Alterar duração — ${editTarget?.servico.nome ?? ''}`
        }
        size="sm"
      >
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              {editTarget?.field === 'preco' ? 'Novo preço (R$)' : 'Nova duração (min)'}
            </label>
            <input
              type="number"
              className="input"
              min={editTarget?.field === 'duracao' ? 1 : 0}
              max={editTarget?.field === 'duracao' ? 480 : undefined}
              step={editTarget?.field === 'preco' ? 0.01 : 1}
              value={editValue}
              onChange={e => setEditValue(e.target.value)}
              autoFocus
            />
          </div>
          <div className="flex justify-end gap-2">
            <button className="btn-secondary" onClick={() => setEditTarget(null)}>Cancelar</button>
            <button className="btn-primary" onClick={handleSaveEdit} disabled={isSaving}>
              {isSaving ? 'Salvando…' : 'Salvar'}
            </button>
          </div>
        </div>
      </Modal>

      {/* ── Desativar serviço ─────────────────────────────────────── */}
      <Modal
        open={!!confirmDeact}
        onClose={() => setConfirmDeact(null)}
        title="Desativar serviço"
        size="sm"
      >
        <div className="space-y-4">
          <p className="text-sm text-gray-700">
            Desativar <span className="font-medium">{confirmDeact?.nome}</span>? O serviço não
            estará disponível para novos agendamentos.
          </p>
          <div className="flex justify-end gap-2">
            <button className="btn-secondary" onClick={() => setConfirmDeact(null)}>Cancelar</button>
            <button
              className="inline-flex items-center justify-center rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 disabled:opacity-50 transition-colors"
              onClick={() => confirmDeact && deactivateMutation.mutate(confirmDeact.id)}
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
