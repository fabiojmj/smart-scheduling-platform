export interface User {
  id: string
  nome: string
  email: string
  role: string
}

export interface Session {
  token: string
  user: User
  estabelecimentoId: string | null
}

export interface Estabelecimento {
  id: string
  nome: string
  whatsAppPhoneNumberId: string
  ativo: boolean
  criadoEm: string
}

export interface Horario {
  diaSemana: number | string
  horaInicio: string
  horaFim: string
}

export interface Funcionario {
  id: string
  nome: string
  email: string
  estabelecimentoId: string
  ativo: boolean
  horarios: Horario[]
  criadoEm: string
}

export interface Servico {
  id: string
  nome: string
  descricao?: string
  duracaoMinutos: number
  preco: number
  estabelecimentoId: string
  ativo: boolean
}

export interface Agendamento {
  id: string
  clienteId: string
  nomeCliente: string
  funcionarioId: string
  nomeFuncionario: string
  servicoId: string
  nomeServico: string
  estabelecimentoId: string
  inicioEm: string
  fimEm: string
  status: string
  observacoes?: string
  motivoCancelamento?: string
  criadoEm: string
}

export interface Cliente {
  id: string
  nome: string
  telefone: string
  email?: string
  estabelecimentoId: string
  criadoEm: string
}

export type Toast = {
  id: number
  message: string
  type: 'success' | 'error' | 'info'
}
