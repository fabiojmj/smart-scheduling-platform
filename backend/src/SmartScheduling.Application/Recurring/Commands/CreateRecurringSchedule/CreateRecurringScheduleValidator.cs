using FluentValidation;
using SmartScheduling.Domain.Enums;

namespace SmartScheduling.Application.Recurring.Commands.CreateRecurringSchedule;

public class CreateRecurringScheduleValidator
    : AbstractValidator<CreateRecurringScheduleCommand>
{
    public CreateRecurringScheduleValidator()
    {
        RuleFor(x => x.EstablishmentId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.ServiceId).NotEmpty();
        RuleFor(x => x.Interval).GreaterThan(0);
        RuleFor(x => x.StartsOn).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));

        RuleFor(x => x.DaysOfWeek)
            .NotEmpty()
            .When(x => x.Frequency == RecurrenceFrequency.Weekly)
            .WithMessage("Informe ao menos um dia da semana para recorrencia semanal.");

        RuleFor(x => x.DayOfMonth)
            .NotNull()
            .InclusiveBetween(1, 28)
            .When(x => x.Frequency == RecurrenceFrequency.Monthly)
            .WithMessage("Dia do mes deve ser entre 1 e 28 para recorrencia mensal.");

        RuleFor(x => x.EndsOn)
            .GreaterThan(x => x.StartsOn)
            .When(x => x.EndsOn.HasValue)
            .WithMessage("Data de termino deve ser apos a data de inicio.");

        RuleFor(x => x.MaxOccurrences)
            .GreaterThan(0)
            .When(x => x.MaxOccurrences.HasValue);
    }
}
