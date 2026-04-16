using FluentValidation;
using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Clientes.Commands.AtualizarCliente;

public record AtualizarNomeClienteCommand(Guid Id, string Nome) : IRequest;

public class AtualizarNomeClienteValidator : AbstractValidator<AtualizarNomeClienteCommand>
{
    public AtualizarNomeClienteValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
    }
}

public sealed class AtualizarNomeClienteHandler(IClienteRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<AtualizarNomeClienteCommand>
{
    public async Task Handle(AtualizarNomeClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Cliente nao encontrado.");
        cliente.AtualizarNome(request.Nome);
        repo.Update(cliente);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
