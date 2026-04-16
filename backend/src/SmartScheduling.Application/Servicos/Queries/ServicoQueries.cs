using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Servicos.Queries;

public record ServicoDto(
    Guid Id,
    string Nome,
    string? Descricao,
    int DuracaoMinutos,
    decimal Preco,
    Guid EstabelecimentoId,
    bool Ativo
);

public record ObterServicoQuery(Guid Id) : IRequest<ServicoDto>;
public record ListarServicosQuery(Guid EstabelecimentoId) : IRequest<IReadOnlyList<ServicoDto>>;

public sealed class ObterServicoHandler(IServicoRepository repo)
    : IRequestHandler<ObterServicoQuery, ServicoDto>
{
    public async Task<ServicoDto> Handle(ObterServicoQuery request, CancellationToken cancellationToken)
    {
        var s = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Servico nao encontrado.");
        return new ServicoDto(s.Id, s.Nome, s.Descricao, s.DuracaoMinutos, s.Preco, s.EstabelecimentoId, s.Ativo);
    }
}

public sealed class ListarServicosHandler(IServicoRepository repo)
    : IRequestHandler<ListarServicosQuery, IReadOnlyList<ServicoDto>>
{
    public async Task<IReadOnlyList<ServicoDto>> Handle(ListarServicosQuery request, CancellationToken cancellationToken)
    {
        var items = await repo.ListarPorEstabelecimentoAsync(request.EstabelecimentoId, cancellationToken);
        return items.Select(s => new ServicoDto(s.Id, s.Nome, s.Descricao, s.DuracaoMinutos, s.Preco, s.EstabelecimentoId, s.Ativo)).ToList();
    }
}
