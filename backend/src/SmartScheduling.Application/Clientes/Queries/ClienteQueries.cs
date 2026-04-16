using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Clientes.Queries;

public record ClienteDto(
    Guid Id,
    string Nome,
    string Telefone,
    string? Email,
    Guid EstabelecimentoId,
    DateTime CriadoEm
);

public record ObterClienteQuery(Guid Id) : IRequest<ClienteDto>;
public record ListarClientesQuery(Guid EstabelecimentoId) : IRequest<IReadOnlyList<ClienteDto>>;

public sealed class ObterClienteHandler(IClienteRepository repo)
    : IRequestHandler<ObterClienteQuery, ClienteDto>
{
    public async Task<ClienteDto> Handle(ObterClienteQuery request, CancellationToken cancellationToken)
    {
        var c = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Cliente nao encontrado.");
        return new ClienteDto(c.Id, c.Nome, c.Telefone.Value, c.Email, c.EstabelecimentoId, c.CreatedAt);
    }
}

public sealed class ListarClientesHandler(IClienteRepository repo)
    : IRequestHandler<ListarClientesQuery, IReadOnlyList<ClienteDto>>
{
    public async Task<IReadOnlyList<ClienteDto>> Handle(ListarClientesQuery request, CancellationToken cancellationToken)
    {
        var items = await repo.ListarPorEstabelecimentoAsync(request.EstabelecimentoId, cancellationToken);
        return items.Select(c => new ClienteDto(c.Id, c.Nome, c.Telefone.Value, c.Email, c.EstabelecimentoId, c.CreatedAt)).ToList();
    }
}
