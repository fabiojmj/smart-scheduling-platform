import { createPortal } from 'react-dom'
import { useToastContext } from '@/contexts/ToastContext'
import type { Toast as ToastType } from '@/types'

const styles: Record<ToastType['type'], string> = {
  success: 'bg-green-600 text-white',
  error:   'bg-red-600 text-white',
  info:    'bg-gray-900 text-white',
}

export function ToastContainer() {
  const { toasts, removeToast } = useToastContext()

  return createPortal(
    <div className="fixed bottom-4 right-4 z-[60] flex flex-col gap-2 pointer-events-none">
      {toasts.map(t => (
        <div
          key={t.id}
          className={`pointer-events-auto flex items-center gap-3 rounded-lg px-4 py-3 text-sm shadow-lg max-w-xs ${styles[t.type]}`}
        >
          <span className="flex-1">{t.message}</span>
          <button
            onClick={() => removeToast(t.id)}
            className="opacity-70 hover:opacity-100 text-xs ml-1"
            aria-label="Fechar notificação"
          >
            ✕
          </button>
        </div>
      ))}
    </div>,
    document.body,
  )
}
