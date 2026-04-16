import { useQuery } from '@tanstack/react-query'
import { api } from '@/lib/api'

export default function EmployeesPage() {
  const { data = [], isLoading } = useQuery({
    queryKey: ['employees'],
    queryFn: () => api.get('/employees').then(r => r.data),
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-xl font-semibold text-gray-900">Funcionários</h2>
        <button className="btn-primary">Novo funcionário</button>
      </div>
      {isLoading ? (
        <p className="text-sm text-gray-500">Carregando...</p>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {data.map((e: any) => (
            <div key={e.id} className="card">
              <div className="flex items-center gap-3 mb-3">
                <div className="w-10 h-10 rounded-full bg-primary-50 flex items-center justify-center text-primary-800 font-medium text-sm">
                  {e.name.charAt(0).toUpperCase()}
                </div>
                <div>
                  <p className="font-medium text-gray-900 text-sm">{e.name}</p>
                  <p className="text-xs text-gray-500">{e.email}</p>
                </div>
              </div>
              <span className={`inline-flex rounded-full px-2 py-0.5 text-xs font-medium ${e.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-500'}`}>
                {e.isActive ? 'Ativo' : 'Inativo'}
              </span>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
