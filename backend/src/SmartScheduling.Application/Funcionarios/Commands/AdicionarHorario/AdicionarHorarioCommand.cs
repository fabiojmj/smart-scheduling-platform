using FluentValidation;
using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Application.Funcionarios.Commands.AdicionarHorario;

public record AdicionarHorarioCommand(
    Guid FuncionarioId,
    DayOfWeek DiaSemana,
    TimeOnly HoraInicio,
    TimeOnly HoraFim
) : IRequest;

public class AdicionarHorarioValidator : AbstractValidator<AdicionarHorarioCommand>
{
    public AdicionarHorarioValidator()
    {
        RuleFor(x => x.FuncionarioId).NotEmpty();
        RuleFor(x => x.HoraFim).GreaterThan(x => x.HoraInicio)
            .WithMessage("Hora de fim deve ser apos a hora de inicio.");
    }
}

public sealed class AdicionarHorarioHandler(IFuncionarioRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<AdicionarHorarioCommand>
{
    public async Task Handle(AdicionarHorarioCommand request, CancellationToken cancellationToken)
    {
        var funcionario = await repo.GetByIdAsync(request.FuncionarioId, cancellationToken)
            ?? throw new DomainException("Funcionario nao encontrado.");
        funcionario.AdicionarHorario(WorkingHours.Create(request.DiaSemana, request.HoraInicio, request.HoraFim));
        repo.Update(funcionario);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
