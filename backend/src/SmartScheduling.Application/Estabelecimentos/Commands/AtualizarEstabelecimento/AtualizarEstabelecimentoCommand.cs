using FluentValidation;
using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Estabelecimentos.Commands.AtualizarEstabelecimento;

public record AtualizarNomeEstabelecimentoCommand(Guid Id, string Nome) : IRequest;
public record DesativarEstabelecimentoCommand(Guid Id) : IRequest;

public class AtualizarNomeEstabelecimentoValidator : AbstractValidator<AtualizarNomeEstabelecimentoCommand>
{
    public AtualizarNomeEstabelecimentoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
    }
}

public sealed class AtualizarNomeEstabelecimentoHandler(IEstabelecimentoRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<AtualizarNomeEstabelecimentoCommand>
{
    public async Task Handle(AtualizarNomeEstabelecimentoCommand request, CancellationToken cancellationToken)
    {
        var estabelecimento = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Estabelecimento nao encontrado.");
        estabelecimento.AtualizarNome(request.Nome);
        repo.Update(estabelecimento);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}

public sealed class DesativarEstabelecimentoHandler(IEstabelecimentoRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<DesativarEstabelecimentoCommand>
{
    public async Task Handle(DesativarEstabelecimentoCommand request, CancellationToken cancellationToken)
    {
        var estabelecimento = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Estabelecimento nao encontrado.");
        estabelecimento.Desativar();
        repo.Update(estabelecimento);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
