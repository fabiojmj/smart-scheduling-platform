import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import LoginPage from '@/pages/LoginPage'
import AppointmentsPage from '@/pages/AppointmentsPage'
import NotFoundPage from '@/pages/NotFoundPage'

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuth()
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/appointments"
        element={
          <PrivateRoute>
            <AppointmentsPage />
          </PrivateRoute>
        }
      />
      <Route path="/" element={<Navigate to="/appointments" replace />} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  )
}
