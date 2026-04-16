using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface IConversaRepository : IRepository<Conversa>
{
    Task<Conversa?> ObterAtivaPorTelefoneAsync(string telefone, Guid estabelecimentoId, CancellationToken cancellationToken = default);
}
