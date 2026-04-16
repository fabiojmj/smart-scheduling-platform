using MediatR;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Application.Appointments.Commands.ScheduleAppointment;

public sealed class ScheduleAppointmentHandler(
    IRepository<Cliente> clienteRepo,
    IRepository<Funcionario> funcionarioRepo,
    IRepository<Servico> servicoRepo,
    IAgendamentoRepository agendamentoRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ScheduleAppointmentCommand, Guid>
{
    public async Task<Guid> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var cliente     = await clienteRepo.GetByIdAsync(request.ClientId, cancellationToken)      ?? throw new DomainException("Cliente nao encontrado.");
        var funcionario = await funcionarioRepo.GetByIdAsync(request.EmployeeId, cancellationToken) ?? throw new DomainException("Funcionario nao encontrado.");
        var servico     = await servicoRepo.GetByIdAsync(request.ServiceId, cancellationToken)      ?? throw new DomainException("Servico nao encontrado.");

        var horario = TimeSlot.Create(request.Start, servico.DuracaoMinutos);
        var agendamento = Agendamento.Agendar(cliente, funcionario, servico, horario, request.Notes);
        agendamento.Confirmar();

        await agendamentoRepo.AddAsync(agendamento, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return agendamento.Id;
    }
}
