import { useQuery } from '@tanstack/react-query'
import { useAuth } from '@/hooks/useAuth'
import { agendamentosApi } from '@/lib/api'
import type { Agendamento } from '@/types'

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

function StatCard({
  label,
  value,
  colorClass = 'text-gray-900',
}: {
  label: string
  value: number | string
  colorClass?: string
}) {
  return (
    <div className="card">
      <p className="text-xs font-medium text-gray-500 uppercase tracking-wide">{label}</p>
      <p className={`text-3xl font-bold mt-1 ${colorClass}`}>{value}</p>
    </div>
  )
}

export default function DashboardPage() {
  const { estabelecimentoId } = useAuth()
  const today = new Date().toISOString().split('T')[0]
  const dataLabel = new Date().toLocaleDateString('pt-BR', {
    weekday: 'long',
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  })

  const { data: agendamentos = [], isLoading } = useQuery<Agendamento[]>({
    queryKey: ['agendamentos', estabelecimentoId, today],
    queryFn: () =>
      agendamentosApi.listarPorEstabelecimento(estabelecimentoId!, today).then(r => r.data),
    enabled: !!estabelecimentoId,
  })

  const sorted = [...agendamentos].sort(
    (a, b) => new Date(a.inicioEm).getTime() - new Date(b.inicioEm).getTime(),
  )

  const total      = agendamentos.length
  const pendentes  = agendamentos.filter(a => a.status === 'Pending').length
  const confirmados = agendamentos.filter(a => a.status === 'Confirmed').length
  const concluidos  = agendamentos.filter(a => a.status === 'Completed').length

  return (
    <div>
      <div className="mb-6">
        <h2 className="text-xl font-semibold text-gray-900">Dashboard</h2>
        <p className="text-sm text-gray-500 mt-0.5 capitalize">{dataLabel}</p>
      </div>

      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        <StatCard label="Total hoje"   value={isLoading ? '…' : total}       />
        <StatCard label="Pendentes"    value={isLoading ? '…' : pendentes}   colorClass="text-yellow-700" />
        <StatCard label="Confirmados"  value={isLoading ? '…' : confirmados} colorClass="text-green-700" />
        <StatCard label="Concluídos"   value={isLoading ? '…' : concluidos}  colorClass="text-blue-700" />
      </div>

      <div className="card overflow-hidden p-0">
        <div className="px-5 py-3 border-b border-gray-100 flex items-center justify-between">
          <h3 className="text-sm font-medium text-gray-900">Agendamentos do dia</h3>
          {!isLoading && <span className="text-xs text-gray-400">{total} total</span>}
        </div>

        {isLoading ? (
          <div className="px-5 py-10 text-center text-sm text-gray-400">Carregando…</div>
        ) : sorted.length === 0 ? (
          <div className="px-5 py-12 text-center text-sm text-gray-400">
            Nenhum agendamento para hoje.
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-100">
              <tr>
                <th className="px-5 py-2.5 text-left text-xs font-medium text-gray-500">Horário</th>
                <th className="px-5 py-2.5 text-left text-xs font-medium text-gray-500">Cliente</th>
                <th className="px-5 py-2.5 text-left text-xs font-medium text-gray-500 hidden sm:table-cell">Serviço</th>
                <th className="px-5 py-2.5 text-left text-xs font-medium text-gray-500 hidden md:table-cell">Funcionário</th>
                <th className="px-5 py-2.5 text-left text-xs font-medium text-gray-500">Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-50">
              {sorted.map(a => (
                <tr key={a.id} className="hover:bg-gray-50">
                  <td className="px-5 py-3 font-medium text-gray-900 tabular-nums whitespace-nowrap">
                    {new Date(a.inicioEm).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}
                  </td>
                  <td className="px-5 py-3 text-gray-700">{a.nomeCliente}</td>
                  <td className="px-5 py-3 text-gray-600 hidden sm:table-cell">{a.nomeServico}</td>
                  <td className="px-5 py-3 text-gray-600 hidden md:table-cell">{a.nomeFuncionario}</td>
                  <td className="px-5 py-3">
                    <StatusBadge status={a.status} />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}
