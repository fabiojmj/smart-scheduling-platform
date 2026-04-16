using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface IFuncionarioRepository : IRepository<Funcionario>
{
    Task<IReadOnlyList<Funcionario>> ListarPorEstabelecimentoAsync(Guid estabelecimentoId, CancellationToken cancellationToken = default);
    Task<Funcionario?> ObterComDetalhesAsync(Guid id, CancellationToken cancellationToken = default);
}
