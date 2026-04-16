import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { useAuth } from '@/hooks/useAuth'

export default function AppointmentsPage() {
  const { logout }     = useAuth()
  const queryClient    = useQueryClient()

  const { data = [], isLoading } = useQuery({
    queryKey: ['my-appointments'],
    queryFn: () => api.get('/client/appointments').then(r => r.data),
  })

  const cancel = useMutation({
    mutationFn: (id: string) => api.delete(`/appointments/${id}`, { data: 'Cancelado pelo cliente' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['my-appointments'] }),
  })

  const upcoming = data.filter((a: any) => a.status === 'Confirmed' || a.status === 'Pending')
  const past     = data.filter((a: any) => a.status === 'Completed' || a.status === 'Cancelled' || a.status === 'NoShow')

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white border-b border-gray-200 px-4 py-4 flex items-center justify-between">
        <h1 className="text-base font-semibold text-gray-900">Meus agendamentos</h1>
        <button onClick={logout} className="text-sm text-gray-500 hover:text-gray-700">Sair</button>
      </header>
      <div className="max-w-lg mx-auto px-4 py-6 space-y-6">
        {isLoading ? (
          <p className="text-sm text-gray-500 text-center">Carregando...</p>
        ) : (
          <>
            <Section title="Próximos">
              {upcoming.length === 0
                ? <p className="text-sm text-gray-400">Nenhum agendamento futuro.</p>
                : upcoming.map((a: any) => (
                    <AppointmentCard key={a.id} appointment={a} onCancel={() => cancel.mutate(a.id)} />
                  ))
              }
            </Section>
            <Section title="Histórico">
              {past.length === 0
                ? <p className="text-sm text-gray-400">Nenhum agendamento anterior.</p>
                : past.map((a: any) => <AppointmentCard key={a.id} appointment={a} />)
              }
            </Section>
          </>
        )}
      </div>
    </div>
  )
}

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div>
      <h2 className="text-sm font-medium text-gray-500 uppercase tracking-wide mb-3">{title}</h2>
      <div className="space-y-3">{children}</div>
    </div>
  )
}

function AppointmentCard({ appointment: a, onCancel }: { appointment: any; onCancel?: () => void }) {
  const date = new Date(a.start)
  return (
    <div className="card">
      <div className="flex items-start justify-between">
        <div>
          <p className="font-medium text-gray-900">{a.serviceName}</p>
          <p className="text-sm text-gray-500 mt-0.5">com {a.employeeName}</p>
          <p className="text-sm text-gray-500">
            {date.toLocaleDateString('pt-BR', { weekday: 'long', day: '2-digit', month: 'long' })}
            {' às '}
            {date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}
          </p>
        </div>
        {onCancel && (
          <button onClick={onCancel} className="text-xs text-red-600 hover:text-red-800 mt-1">Cancelar</button>
        )}
      </div>
    </div>
  )
}
