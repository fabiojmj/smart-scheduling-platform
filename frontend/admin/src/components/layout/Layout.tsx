import { Outlet, NavLink } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'

const navItems = [
  { to: '/dashboard',    label: 'Dashboard' },
  { to: '/appointments', label: 'Agendamentos' },
  { to: '/employees',    label: 'Funcionários' },
  { to: '/services',     label: 'Serviços' },
]

export default function Layout() {
  const { logout } = useAuth()

  return (
    <div className="flex h-screen bg-gray-50">
      <aside className="w-64 border-r border-gray-200 bg-white flex flex-col">
        <div className="px-6 py-5 border-b border-gray-200">
          <h1 className="text-base font-semibold text-gray-900">Smart Scheduling</h1>
          <p className="text-xs text-gray-500 mt-0.5">Painel administrativo</p>
        </div>
        <nav className="flex-1 px-3 py-4 space-y-1">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                `flex items-center px-3 py-2 rounded-lg text-sm transition-colors ${
                  isActive
                    ? 'bg-primary-50 text-primary-800 font-medium'
                    : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'
                }`
              }
            >
              {item.label}
            </NavLink>
          ))}
        </nav>
        <div className="px-3 py-4 border-t border-gray-200">
          <button
            onClick={logout}
            className="w-full flex items-center px-3 py-2 rounded-lg text-sm text-gray-600 hover:bg-gray-100 hover:text-gray-900 transition-colors"
          >
            Sair
          </button>
        </div>
      </aside>
      <main className="flex-1 overflow-auto">
        <div className="max-w-7xl mx-auto px-6 py-8">
          <Outlet />
        </div>
      </main>
    </div>
  )
}
