using MediatR;

namespace SmartScheduling.Application.Servicos.Commands.CriarServico;

public record CriarServicoCommand(
    string Nome,
    int DuracaoMinutos,
    decimal Preco,
    Guid EstabelecimentoId,
    string? Descricao = null
) : IRequest<Guid>;
