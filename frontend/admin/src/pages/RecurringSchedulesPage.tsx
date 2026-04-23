import { useState } from 'react'
import {
  useRecurringSchedules,
  useCancelRecurringSchedule,
  useCreateRecurringSchedule,
  type CreateRecurringSchedulePayload,
} from '@/hooks/useRecurringSchedules'

const ESTABLISHMENT_ID = import.meta.env.VITE_ESTABLISHMENT_ID ?? ''

const FREQ_LABELS: Record<number, string> = { 1: 'Diario', 2: 'Semanal', 3: 'Mensal' }
const DAY_LABELS  = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab']
const DAYS        = [1, 2, 3, 4, 5, 6, 0]

const empty: CreateRecurringSchedulePayload = {
  clientId: '',
  employeeId: '',
  serviceId: '',
  frequency: 2,
  interval: 1,
  daysOfWeek: [1],
  dayOfMonth: null,
  startTime: '09:00',
  startsOn: new Date().toISOString().split('T')[0],
  endsOn: null,
  maxOccurrences: null,
}

export default function RecurringSchedulesPage() {
  const { data = [], isLoading }  = useRecurringSchedules(ESTABLISHMENT_ID)
  const cancel                    = useCancelRecurringSchedule(ESTABLISHMENT_ID)
  const create                    = useCreateRecurringSchedule(ESTABLISHMENT_ID)
  const [showForm, setShowForm]   = useState(false)
  const [form, setForm]           = useState<CreateRecurringSchedulePayload>(empty)

  function toggleDay(day: number) {
    setForm(f => ({
      ...f,
      daysOfWeek: f.daysOfWeek.includes(day)
        ? f.daysOfWeek.filter(d => d !== day)
        : [...f.daysOfWeek, day],
    }))
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    await create.mutateAsync(form)
    setShowForm(false)
    setForm(empty)
  }

  return (
    <div>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 20 }}>
        <div>
          <h2 style={{ fontSize: 16, fontWeight: 500, margin: 0 }}>Recorrencias</h2>
          <p style={{ fontSize: 12, color: 'var(--color-text-tertiary)', marginTop: 2 }}>
            Agendamentos automaticos que se repetem periodicamente
          </p>
        </div>
        <button
          onClick={() => setShowForm(s => !s)}
          style={{ fontSize: 12, padding: '6px 14px', borderRadius: 8, border: 'none', background: '#1D9E75', color: '#fff', cursor: 'pointer' }}
        >
          {showForm ? 'Cancelar' : '+ Nova recorrencia'}
        </button>
      </div>

      {showForm && (
        <div style={{ background: 'var(--color-background-primary)', border: '0.5px solid var(--color-border-tertiary)', borderRadius: 12, padding: 20, marginBottom: 20 }}>
          <form onSubmit={handleSubmit}>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12, marginBottom: 12 }}>
              <div>
                <label style={{ fontSize: 11, fontWeight: 500, color: 'var(--color-text-secondary)', display: 'block', marginBottom: 4 }}>ID do cliente</label>
                <input style={{ width: '100%', fontSize: 12, padding: '6px 10px', border: '0.5px solid var(--color-border-secondary)', borderRadius: 8, background: 'var(--color-background-primary)', color: 'var(--color-text-primary)' }}
                  value={form.clientId} onChange={e => setForm(f => ({ ...f, clientId: e.target.value }))} placeholder="UUID do cliente" required />
              </div>
              <div>
                <label style={{ fontSize: 11, fontWeight: 500, color: 'var(--color-text-secondary)', display: 'block', marginBottom: 4 }}>ID do funcionario</label>
                <input style={{ width: '100%', fontSize: 12, padding: '6px 10px', border: '0.5px solid var(--color-border-secondary)', borderRadius: 8, background: 'var(--color-background-primary)', color: 'var(--color-text-primary)' }}
                  value={form.employeeId} onChange={e => setForm(f => ({ ...f, employeeId: e.target.value }))} placeholder="UUID do funcionario" required />
              </div>
              <div>
                <label style={{ fontSize: 11, fontWeight: 500, color: 'var(--color-text-secondary)', display: 'block', marginBottom: 4 }}>ID do servico</label>
                <input style={{ width: '100%', fontSize: 12, padding: '6px 10px', border: '0.5px solid var(--color-border-secondary)', borderRadius: 8, background: 'var(--color-background-primary)', color: 'var(--color-text-primary)' }}
                  value={form.serviceId} onChange={e => setForm(f => ({ ...f, serviceId: e.target.value }))} placeholder="UUID do servico" required />
              </div>
              <div>
                <label style={{ fontSize: 11, fontWeight: 500, color: 'var(--color-text-secondary)', display: 'block', marginBottom: 4 }}>Frequencia</label>
                <select style={{ width: '100%', fontSize: 12, padding: '6px 10px', border: '0.5px solid var(--color-border-secondary)', borderRadius: 8, background: 'var(--color-background-primary)', color: 'var(--color-text-primary)' }}
                  value={form.frequency} onChange={e => setForm(f => ({ ...f, frequency: Number(e.target.value) as 1|2|3 }))}>
                  <option value={2}>Semanal</option>
                  <option value={1}>Diario</option>
                  <option value={3}>Mensal</option>
                </select>
              </div>
            </div>

            {form.frequency === 2 && (
              <div style={{ marginBottom: 12 }}>
                <label style={{ fontSize: 11, fontWeight: 500, color: 'var(--color-text-secondary)', display: 'block', marginBottom: 6 }}>Dias da semana</label>
                <div style={{ display: 'flex', gap: 6 }}>
                  {DAYS.map(d => (
                    <button key={d} type="button" onClick={() => toggleDay(d)}
                      style={{ width: 36, height: 36, borderRadius: '50%', border: '0.5px solid var(--color-border-secondary)', fontSize: 11, cursor: 'pointer', fontWeight: form.daysOfWeek.includes(d) ? 500 : 400, background: form.daysOfWeek.includes(d) ? '#1D9E75' : 'var(--color-background-primary)', color: form.daysOfWeek.includes(d) ? '#fff' : 'var(--color-text-secondary)' }}>
                      {DAY_LABELS[d]}
                    </button>
                  ))}
                </div>
              </div>
            )}

            {form.frequency === 3 && (
              <div style={{ marginBottom: 12 }}>
                <label style={{ fontSize: 11, fontWeight: 500, color: 'var(--color-text-secondary)', display: 'block', marginBottom: 4 }}>Dia do mes</label>
                <input type="number" min={1} max={28} style={{ fontSize: 12, padding: '6px 10px', border: '0.5px solid var(--color-border-secondary)', borderRadius: 8, background: 'var(--color-background-primary)', color: 'var(--color-text-primary)', width: 80 }}
                  value={form.dayOfMonth ?? ''} onChange={e => setForm(f => ({ ...f, dayOfMonth: Number(e.target.value) }))} required />
              </div>
            )}

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: 12, marginBottom: 16 }}>
              <div>
                <label style={{ fontSize: 11, fontWeight: 500, color: 'var(--color-text-secondary)', display: 'block', marginBottom: 4 }}>Horario</label>
                <input type="time" style={{ fontSize: 12, padding: '6px 10px', border: '0.5px solid var(--color-border-secondary)', borderRadius: 8, background: 'var(--color-background-primary)', color: 'var(--color-text-primary)', width: '100%' }}
                  value={form.startTime} onChange={e => setForm(f => ({ ...f, startTime: e.target.value }))} required />
              </div>
              <div>
                <label style={{ fontSize: 11, fontWeight: 500, color: 'var(--color-text-secondary)', display: 'block', marginBottom: 4 }}>Inicio</label>
                <input type="date" style={{ fontSize: 12, padding: '6px 10px', border: '0.5px solid var(--color-border-secondary)', borderRadius: 8, background: 'var(--color-background-primary)', color: 'var(--color-text-primary)', width: '100%' }}
                  value={form.startsOn} onChange={e => setForm(f => ({ ...f, startsOn: e.target.value }))} required />
              </div>
              <div>
                <label style={{ fontSize: 11, fontWeight: 500, color: 'var(--color-text-secondary)', display: 'block', marginBottom: 4 }}>Termino (opcional)</label>
                <input type="date" style={{ fontSize: 12, padding: '6px 10px', border: '0.5px solid var(--color-border-secondary)', borderRadius: 8, background: 'var(--color-background-primary)', color: 'var(--color-text-primary)', width: '100%' }}
                  value={form.endsOn ?? ''} onChange={e => setForm(f => ({ ...f, endsOn: e.target.value || null }))} />
              </div>
            </div>

            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8 }}>
              <button type="button" onClick={() => setShowForm(false)}
                style={{ fontSize: 12, padding: '6px 14px', borderRadius: 8, border: '0.5px solid var(--color-border-secondary)', background: 'transparent', color: 'var(--color-text-secondary)', cursor: 'pointer' }}>
                Cancelar
              </button>
              <button type="submit" disabled={create.isPending}
                style={{ fontSize: 12, padding: '6px 14px', borderRadius: 8, border: 'none', background: '#1D9E75', color: '#fff', cursor: 'pointer' }}>
                {create.isPending ? 'Criando...' : 'Criar recorrencia'}
              </button>
            </div>
          </form>
        </div>
      )}

      {isLoading ? (
        <p style={{ fontSize: 12, color: 'var(--color-text-tertiary)' }}>Carregando...</p>
      ) : data.length === 0 ? (
        <div style={{ textAlign: 'center', padding: '48px 0', color: 'var(--color-text-tertiary)', fontSize: 13 }}>
          Nenhuma recorrencia cadastrada ainda.
        </div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
          {data.map(s => (
            <div key={s.id} style={{ background: 'var(--color-background-primary)', border: '0.5px solid var(--color-border-tertiary)', borderRadius: 12, padding: '12px 16px', display: 'flex', alignItems: 'center', gap: 12 }}>
              <div style={{ width: 8, height: 8, borderRadius: '50%', background: s.isActive ? '#1D9E75' : '#B4B2A9', flexShrink: 0, marginTop: 2 }} />
              <div style={{ flex: 1 }}>
                <div style={{ fontSize: 13, fontWeight: 500, color: 'var(--color-text-primary)' }}>
                  {s.clientName} — {s.serviceName}
                </div>
                <div style={{ fontSize: 11, color: 'var(--color-text-tertiary)', marginTop: 2 }}>
                  {s.description} &middot; com {s.employeeName}
                  {s.endsOn && ` · ate ${new Date(s.endsOn).toLocaleDateString('pt-BR')}`}
                </div>
              </div>
              <span style={{ fontSize: 10, padding: '2px 8px', borderRadius: 999, background: FREQ_LABELS[s.frequency] === 'Semanal' ? '#E1F5EE' : '#FAEEDA', color: FREQ_LABELS[s.frequency] === 'Semanal' ? '#085041' : '#633806', fontWeight: 500, flexShrink: 0 }}>
                {FREQ_LABELS[s.frequency]}
              </span>
              {s.isActive && (
                <button onClick={() => cancel.mutate(s.id)}
                  style={{ fontSize: 11, padding: '4px 10px', borderRadius: 6, border: '0.5px solid var(--color-border-secondary)', background: 'transparent', color: 'var(--color-text-danger)', cursor: 'pointer', flexShrink: 0 }}>
                  Cancelar
                </button>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
