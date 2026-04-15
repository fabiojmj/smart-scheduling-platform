using MediatR;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Application.Appointments.Commands.ScheduleAppointment;

public sealed class ScheduleAppointmentHandler(
    IRepository<Client> clientRepo, IRepository<Employee> employeeRepo,
    IRepository<Service> serviceRepo, IAppointmentRepository appointmentRepo, IUnitOfWork unitOfWork)
    : IRequestHandler<ScheduleAppointmentCommand, Guid>
{
    public async Task<Guid> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var client   = await clientRepo.GetByIdAsync(request.ClientId, cancellationToken)    ?? throw new DomainException("Cliente nao encontrado.");
        var employee = await employeeRepo.GetByIdAsync(request.EmployeeId, cancellationToken) ?? throw new DomainException("Funcionario nao encontrado.");
        var service  = await serviceRepo.GetByIdAsync(request.ServiceId, cancellationToken)   ?? throw new DomainException("Servico nao encontrado.");
        var slot = TimeSlot.Create(request.Start, service.DurationMinutes);
        var appointment = Appointment.Schedule(client, employee, service, slot, request.Notes);
        appointment.Confirm();
        await appointmentRepo.AddAsync(appointment, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return appointment.Id;
    }
}
