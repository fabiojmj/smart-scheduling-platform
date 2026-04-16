using MediatR;

namespace SmartScheduling.Application.Funcionarios.Commands.CriarFuncionario;

public record CriarFuncionarioCommand(
    string Nome,
    string Email,
    Guid EstabelecimentoId
) : IRequest<Guid>;
