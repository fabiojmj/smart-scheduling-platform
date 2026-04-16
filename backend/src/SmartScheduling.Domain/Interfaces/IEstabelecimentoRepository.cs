using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface IEstabelecimentoRepository : IRepository<Estabelecimento>
{
    Task<Estabelecimento?> ObterPorProprietarioAsync(string proprietarioId, CancellationToken cancellationToken = default);
}
