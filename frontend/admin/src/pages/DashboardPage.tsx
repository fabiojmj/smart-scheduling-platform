import { useQuery } from '@tanstack/react-query'
import { api } from '@/lib/api'

export default function DashboardPage() {
  const today = new Date().toISOString().split('T')[0]

  const { data, isLoading } = useQuery({
    queryKey: ['dashboard', today],
    queryFn: () => api.get(`/appointments/today`).then(r => r.data),
  })

  return (
    <div>
      <h2 className="text-xl font-semibold text-gray-900 mb-6">Dashboard</h2>
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-8">
        <StatCard label="Agendamentos hoje" value={isLoading ? '...' : data?.total ?? 0} />
        <StatCard label="Confirmados"        value={isLoading ? '...' : data?.confirmed ?? 0} />
        <StatCard label="Cancelados"         value={isLoading ? '...' : data?.cancelled ?? 0} />
      </div>
    </div>
  )
}

function StatCard({ label, value }: { label: string; value: number | string }) {
  return (
    <div className="card">
      <p className="text-sm text-gray-500">{label}</p>
      <p className="text-3xl font-semibold text-gray-900 mt-1">{value}</p>
    </div>
  )
}
