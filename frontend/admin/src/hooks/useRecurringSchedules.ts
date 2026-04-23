import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'

export type RecurrenceFrequency = 1 | 2 | 3

export interface RecurringScheduleDto {
  id: string
  clientName: string
  employeeName: string
  serviceName: string
  frequency: RecurrenceFrequency
  interval: number
  daysOfWeek: number[]
  dayOfMonth: number | null
  startTime: string
  startsOn: string
  endsOn: string | null
  maxOccurrences: number | null
  isActive: boolean
  description: string
}

export interface CreateRecurringSchedulePayload {
  clientId: string
  employeeId: string
  serviceId: string
  frequency: RecurrenceFrequency
  interval: number
  daysOfWeek: number[]
  dayOfMonth: number | null
  startTime: string
  startsOn: string
  endsOn: string | null
  maxOccurrences: number | null
}

export function useRecurringSchedules(establishmentId: string) {
  return useQuery<RecurringScheduleDto[]>({
    queryKey: ['recurring-schedules', establishmentId],
    queryFn: () =>
      api
        .get(`/establishments/${establishmentId}/recurring-schedules`)
        .then(r => r.data),
    enabled: !!establishmentId,
  })
}

export function useCreateRecurringSchedule(establishmentId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (payload: CreateRecurringSchedulePayload) =>
      api
        .post(`/establishments/${establishmentId}/recurring-schedules`, payload)
        .then(r => r.data),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: ['recurring-schedules', establishmentId],
      }),
  })
}

export function useCancelRecurringSchedule(establishmentId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) =>
      api.delete(`/establishments/${establishmentId}/recurring-schedules/${id}`),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: ['recurring-schedules', establishmentId],
      }),
  })
}
