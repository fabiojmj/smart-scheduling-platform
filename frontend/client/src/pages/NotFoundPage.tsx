import { Link } from 'react-router-dom'

export default function NotFoundPage() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="text-center">
        <p className="text-6xl font-bold text-gray-200">404</p>
        <p className="mt-4 text-gray-600">Página não encontrada.</p>
        <Link to="/appointments" className="mt-6 inline-block btn-primary">Voltar</Link>
      </div>
    </div>
  )
}
