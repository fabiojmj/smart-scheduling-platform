using MediatR;

namespace SmartScheduling.Application.Recurring.Commands.CancelRecurringSchedule;

public record CancelRecurringScheduleCommand(Guid RecurringScheduleId) : IRequest;
