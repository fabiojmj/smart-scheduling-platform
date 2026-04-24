import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import Layout from '@/components/layout/Layout'
import LoginPage from '@/pages/LoginPage'
import DashboardPage from '@/pages/DashboardPage'
import AppointmentsPage from '@/pages/AppointmentsPage'
import EmployeesPage from '@/pages/EmployeesPage'
import ServicesPage from '@/pages/ServicesPage'
import ClientesPage from '@/pages/ClientesPage'
import EstabelecimentoPage from '@/pages/EstabelecimentoPage'
import NotFoundPage from '@/pages/NotFoundPage'

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isReady } = useAuth()
  if (!isReady) return <div className="min-h-screen bg-gray-50" />
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/"
        element={
          <PrivateRoute>
            <Layout />
          </PrivateRoute>
        }
      >
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="dashboard"    element={<DashboardPage />} />
        <Route path="appointments" element={<AppointmentsPage />} />
        <Route path="employees"    element={<EmployeesPage />} />
        <Route path="services"     element={<ServicesPage />} />
        <Route path="clients"      element={<ClientesPage />} />
        <Route path="settings"     element={<EstabelecimentoPage />} />
      </Route>
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  )
}
