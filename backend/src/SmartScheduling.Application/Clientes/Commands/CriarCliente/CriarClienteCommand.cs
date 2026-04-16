using MediatR;

namespace SmartScheduling.Application.Clientes.Commands.CriarCliente;

public record CriarClienteCommand(
    string Nome,
    string Telefone,
    Guid EstabelecimentoId,
    string? Email = null
) : IRequest<Guid>;
