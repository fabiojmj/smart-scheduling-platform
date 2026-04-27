import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useAuth } from '@/hooks/useAuth'
import { useToast } from '@/hooks/useToast'
import { estabelecimentosApi, funcionariosApi, extractError } from '@/lib/api'
import { Modal } from '@/components/ui/Modal'
import type { Estabelecimento, Funcionario } from '@/types'

export default function EstabelecimentoPage() {
  const { estabelecimentoId } = useAuth()
  const toast = useToast()
  const queryClient = useQueryClient()

  const [editOpen, setEditOpen]   = useState(false)
  const [newNome, setNewNome]     = useState('')
  const [nomeError, setNomeError] = useState('')

  const { data: estabelecimento, isLoading } = useQuery<Estabelecimento>({
    queryKey: ['estabelecimento', estabelecimentoId],
    queryFn:  () => estabelecimentosApi.meu().then(r => r.data),
    enabled:  !!estabelecimentoId,
  })

  const { data: funcionarios = [] } = useQuery<Funcionario[]>({
    queryKey: ['funcionarios', estabelecimentoId],
    queryFn:  () => funcionariosApi.listar(estabelecimentoId!).then(r => r.data),
    enabled:  !!estabelecimentoId,
  })

  const funcionariosAtivos = funcionarios.filter(f => f.ativo)

  const updateMutation = useMutation({
    mutationFn: ({ id, nome }: { id: string; nome: string }) =>
      estabelecimentosApi.atualizarNome(id, nome),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['estabelecimento'] })
      toast.success('Nome atualizado.')
      setEditOpen(false)
    },
    onError: err => toast.error(extractError(err)),
  })

  const primeiroAtendimentoMutation = useMutation({
    mutationFn: ({ id, funcionarioId }: { id: string; funcionarioId: string | null }) =>
      estabelecimentosApi.definirPrimeiroAtendimento(id, funcionarioId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['estabelecimento'] })
      toast.success('Configuração de primeiro atendimento salva.')
    },
    onError: err => toast.error(extractError(err)),
  })

  function handleUpdate() {
    const nome = newNome.trim()
    if (!nome || nome.length < 2) {
      setNomeError('Nome deve ter pelo menos 2 caracteres.')
      return
    }
    setNomeError('')
    updateMutation.mutate({ id: estabelecimentoId!, nome })
  }

  function handlePrimeiroAtendimento(funcionarioId: string) {
    primeiroAtendimentoMutation.mutate({
      id: estabelecimentoId!,
      funcionarioId: funcionarioId === '' ? null : funcionarioId,
    })
  }

  if (!estabelecimentoId) {
    return (
      <div>
        <h2 className="text-xl font-semibold text-gray-900 mb-6">Configurações</h2>
        <div className="card max-w-md text-center py-12">
          <p className="text-gray-500 text-sm">
            Nenhum estabelecimento vinculado a esta conta.
            Entre em contato com o administrador do sistema.
          </p>
        </div>
      </div>
    )
  }

  return (
    <div>
      <h2 className="text-xl font-semibold text-gray-900 mb-6">Configurações</h2>

      {isLoading ? (
        <p className="text-sm text-gray-500">Carregando…</p>
      ) : estabelecimento ? (
        <div className="max-w-lg space-y-4">
          <div className="card">
            <div className="flex items-start justify-between mb-4">
              <div>
                <p className="text-xs font-medium text-gray-500 uppercase tracking-wide mb-1">
                  Estabelecimento
                </p>
                <p className="text-xl font-semibold text-gray-900">{estabelecimento.nome}</p>
              </div>
              <button
                className="btn-secondary text-xs py-1.5"
                onClick={() => {
                  setNewNome(estabelecimento.nome)
                  setNomeError('')
                  setEditOpen(true)
                }}
              >
                Editar nome
              </button>
            </div>

            <dl className="space-y-3 text-sm">
              <div className="flex items-center justify-between py-2 border-t border-gray-100">
                <dt className="text-gray-500">Status</dt>
                <dd>
                  <span
                    className={`inline-flex rounded-full px-2 py-0.5 text-xs font-medium ${
                      estabelecimento.ativo
                        ? 'bg-green-100 text-green-800'
                        : 'bg-red-100 text-red-700'
                    }`}
                  >
                    {estabelecimento.ativo ? 'Ativo' : 'Inativo'}
                  </span>
                </dd>
              </div>
              <div className="flex items-center justify-between py-2 border-t border-gray-100">
                <dt className="text-gray-500">WhatsApp Phone Number ID</dt>
                <dd>
                  <code className="text-xs bg-gray-50 border border-gray-200 rounded px-2 py-0.5 font-mono text-gray-700">
                    {estabelecimento.whatsAppPhoneNumberId}
                  </code>
                </dd>
              </div>
              <div className="flex items-center justify-between py-2 border-t border-gray-100">
                <dt className="text-gray-500">Cadastrado em</dt>
                <dd className="text-gray-700">
                  {new Date(estabelecimento.criadoEm).toLocaleDateString('pt-BR', {
                    day: '2-digit',
                    month: 'long',
                    year: 'numeric',
                  })}
                </dd>
              </div>
            </dl>
          </div>

          {funcionariosAtivos.length > 1 && (
            <div className="card">
              <p className="text-xs font-medium text-gray-500 uppercase tracking-wide mb-1">
                Primeiro Atendimento
              </p>
              <p className="text-sm text-gray-600 mb-3">
                Selecione o funcionário responsável pelo atendimento de novos clientes via chatbot.
              </p>
              <div className="flex items-center gap-3">
                <select
                  className="input flex-1"
                  value={estabelecimento.funcionarioIdPrimeiroAtendimento ?? ''}
                  onChange={e => handlePrimeiroAtendimento(e.target.value)}
                  disabled={primeiroAtendimentoMutation.isPending}
                >
                  <option value="">— Sem responsável definido —</option>
                  {funcionariosAtivos.map(f => (
                    <option key={f.id} value={f.id}>{f.nome}</option>
                  ))}
                </select>
                {primeiroAtendimentoMutation.isPending && (
                  <span className="text-xs text-gray-400">Salvando…</span>
                )}
              </div>
            </div>
          )}

          <div className="card bg-amber-50 border-amber-200">
            <p className="text-xs font-medium text-amber-800 mb-1">Integração WhatsApp</p>
            <p className="text-xs text-amber-700">
              O WhatsApp Phone Number ID é configurado pela equipe técnica.
              Para alterações, entre em contato com o suporte.
            </p>
          </div>
        </div>
      ) : (
        <p className="text-sm text-gray-400">Nenhum dado encontrado.</p>
      )}

      <Modal open={editOpen} onClose={() => setEditOpen(false)} title="Editar nome" size="sm">
        <div className="space-y-4">
          <div>
            <label htmlFor="est-nome" className="block text-sm font-medium text-gray-700 mb-1">
              Nome do estabelecimento
            </label>
            <input
              id="est-nome"
              type="text"
              className="input"
              value={newNome}
              onChange={e => setNewNome(e.target.value)}
              maxLength={100}
              autoFocus
            />
            {nomeError && <p className="text-xs text-red-600 mt-1" role="alert">{nomeError}</p>}
          </div>
          <div className="flex justify-end gap-2">
            <button className="btn-secondary" onClick={() => setEditOpen(false)}>Cancelar</button>
            <button
              className="btn-primary"
              onClick={handleUpdate}
              disabled={updateMutation.isPending}
            >
              {updateMutation.isPending ? 'Salvando…' : 'Salvar'}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  )
}
