using Microsoft.EntityFrameworkCore;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Persistence.Repositories;

public class ConversaRepository(AppDbContext context) : Repository<Conversa>(context), IConversaRepository
{
    public async Task<Conversa?> ObterAtivaPorTelefoneAsync(string telefone, Guid estabelecimentoId, CancellationToken cancellationToken = default) =>
        await Context.Conversas
            .Include(c => c.Mensagens)
            .FirstOrDefaultAsync(c =>
                c.TelefoneCliente.Value == telefone
                && c.EstabelecimentoId == estabelecimentoId
                && (c.Status == StatusConversa.Ativa || c.Status == StatusConversa.AguardandoConfirmacao),
                cancellationToken);
}
