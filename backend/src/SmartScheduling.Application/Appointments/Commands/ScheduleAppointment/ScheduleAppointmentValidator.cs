using FluentValidation;

namespace SmartScheduling.Application.Appointments.Commands.ScheduleAppointment;

public class ScheduleAppointmentValidator : AbstractValidator<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.ServiceId).NotEmpty();
        RuleFor(x => x.Start).GreaterThan(DateTime.UtcNow).WithMessage("Data do agendamento deve ser no futuro.");
    }
}
