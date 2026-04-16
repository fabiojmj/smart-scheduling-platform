import { useQuery } from '@tanstack/react-query'
import { api } from '@/lib/api'

export default function AppointmentsPage() {
  const today = new Date().toISOString().split('T')[0]

  const { data = [], isLoading } = useQuery({
    queryKey: ['appointments', today],
    queryFn: () => api.get(`/appointments?date=${today}`).then(r => r.data),
  })

  return (
    <div>
      <h2 className="text-xl font-semibold text-gray-900 mb-6">Agendamentos</h2>
      {isLoading ? (
        <p className="text-sm text-gray-500">Carregando...</p>
      ) : (
        <div className="card overflow-hidden p-0">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-4 py-3 text-left font-medium text-gray-600">Cliente</th>
                <th className="px-4 py-3 text-left font-medium text-gray-600">Serviço</th>
                <th className="px-4 py-3 text-left font-medium text-gray-600">Funcionário</th>
                <th className="px-4 py-3 text-left font-medium text-gray-600">Horário</th>
                <th className="px-4 py-3 text-left font-medium text-gray-600">Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {data.map((a: any) => (
                <tr key={a.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3">{a.clientName}</td>
                  <td className="px-4 py-3">{a.serviceName}</td>
                  <td className="px-4 py-3">{a.employeeName}</td>
                  <td className="px-4 py-3">{new Date(a.start).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}</td>
                  <td className="px-4 py-3"><StatusBadge status={a.status} /></td>
                </tr>
              ))}
              {data.length === 0 && (
                <tr><td colSpan={5} className="px-4 py-8 text-center text-gray-400">Nenhum agendamento hoje.</td></tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

const statusMap: Record<string, { label: string; className: string }> = {
  Pending:   { label: 'Pendente',   className: 'bg-yellow-100 text-yellow-800' },
  Confirmed: { label: 'Confirmado', className: 'bg-green-100 text-green-800' },
  Completed: { label: 'Concluído',  className: 'bg-blue-100 text-blue-800' },
  Cancelled: { label: 'Cancelado',  className: 'bg-red-100 text-red-800' },
  NoShow:    { label: 'Não veio',   className: 'bg-gray-100 text-gray-600' },
}

function StatusBadge({ status }: { status: string }) {
  const s = statusMap[status] ?? { label: status, className: 'bg-gray-100 text-gray-600' }
  return <span className={`inline-flex rounded-full px-2 py-0.5 text-xs font-medium ${s.className}`}>{s.label}</span>
}
