import axios from 'axios'

export const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.request.use(config => {
  try {
    const stored = localStorage.getItem('session')
    if (stored) {
      const session = JSON.parse(stored) as { token?: string }
      if (session.token) config.headers.Authorization = `Bearer ${session.token}`
    }
  } catch { /* session corrupted */ }
  return config
})

api.interceptors.response.use(
  res => res,
  err => {
    if (err.response?.status === 401) {
      localStorage.removeItem('session')
      window.location.href = '/login'
    }
    return Promise.reject(err)
  },
)

function extractError(err: unknown): string {
  if (axios.isAxiosError(err)) {
    const data = err.response?.data
    if (typeof data === 'string') return data
    if (data?.detail) return data.detail
    if (data?.title) return data.title
    if (data?.errors) {
      const msgs = Object.values(data.errors as Record<string, string[]>).flat()
      if (msgs.length) return msgs[0]
    }
  }
  return 'Ocorreu um erro inesperado.'
}

// ─── Auth ────────────────────────────────────────────────────────────────────

export const authApi = {
  login: (email: string, senha: string) =>
    api.post<{ token: string; nome: string; email: string; role: string }>('/auth/login', { email, senha }),

  registrar: (nome: string, email: string, senha: string) =>
    api.post<string>('/auth/registrar', { nome, email, senha }),
}

// ─── Estabelecimentos ────────────────────────────────────────────────────────

export const estabelecimentosApi = {
  meu: () =>
    api.get<{ id: string; nome: string; whatsAppPhoneNumberId: string; ativo: boolean; criadoEm: string }>('/estabelecimentos/meu'),

  criar: (nome: string, whatsAppPhoneNumberId: string, proprietarioId: string) =>
    api.post<string>('/estabelecimentos', { nome, whatsAppPhoneNumberId, proprietarioId }),

  atualizarNome: (id: string, nome: string) =>
    api.patch(`/estabelecimentos/${id}`, { nome }),
}

// ─── Funcionários ────────────────────────────────────────────────────────────

export const funcionariosApi = {
  listar: (estabelecimentoId: string) =>
    api.get(`/funcionarios?estabelecimentoId=${estabelecimentoId}`),

  criar: (nome: string, email: string, estabelecimentoId: string) =>
    api.post<string>('/funcionarios', { nome, email, estabelecimentoId }),

  desativar: (id: string) =>
    api.delete(`/funcionarios/${id}`),

  adicionarHorario: (id: string, diaSemana: number, horaInicio: string, horaFim: string) =>
    api.post(`/funcionarios/${id}/horarios`, { diaSemana, horaInicio, horaFim }),

  atribuirServico: (id: string, servicoId: string) =>
    api.post(`/funcionarios/${id}/servicos`, { servicoId }),
}

// ─── Serviços ────────────────────────────────────────────────────────────────

export const servicosApi = {
  listar: (estabelecimentoId: string) =>
    api.get(`/servicos?estabelecimentoId=${estabelecimentoId}`),

  criar: (nome: string, duracaoMinutos: number, preco: number, estabelecimentoId: string, descricao?: string) =>
    api.post<string>('/servicos', { nome, duracaoMinutos, preco, estabelecimentoId, descricao }),

  atualizarPreco: (id: string, novoPreco: number) =>
    api.patch(`/servicos/${id}/preco`, { novoPreco }),

  atualizarDuracao: (id: string, novosDuracaoMinutos: number) =>
    api.patch(`/servicos/${id}/duracao`, { novosDuracaoMinutos }),

  desativar: (id: string) =>
    api.delete(`/servicos/${id}`),
}

// ─── Clientes ────────────────────────────────────────────────────────────────

export const clientesApi = {
  listar: (estabelecimentoId: string) =>
    api.get(`/clientes?estabelecimentoId=${estabelecimentoId}`),

  criar: (nome: string, telefone: string, estabelecimentoId: string, email?: string) =>
    api.post<string>('/clientes', { nome, telefone, estabelecimentoId, email }),
}

// ─── Agendamentos ────────────────────────────────────────────────────────────

export const agendamentosApi = {
  listarPorEstabelecimento: (estabelecimentoId: string, data: string) =>
    api.get(`/agendamentos/por-estabelecimento?estabelecimentoId=${estabelecimentoId}&data=${data}`),

  criar: (clientId: string, employeeId: string, serviceId: string, start: string, notes?: string) =>
    api.post<string>('/agendamentos', { clientId, employeeId, serviceId, start, notes }),

  confirmar: (id: string) =>
    api.put(`/agendamentos/${id}/confirmar`),

  concluir: (id: string) =>
    api.put(`/agendamentos/${id}/concluir`),

  cancelar: (id: string, motivo: string) =>
    api.put(`/agendamentos/${id}/cancelar`, { motivo }),

  naoCompareceu: (id: string) =>
    api.put(`/agendamentos/${id}/nao-compareceu`),
}

export { extractError }
