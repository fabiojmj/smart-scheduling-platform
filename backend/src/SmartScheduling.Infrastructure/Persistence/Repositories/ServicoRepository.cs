using Microsoft.EntityFrameworkCore;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Persistence.Repositories;

public class ServicoRepository(AppDbContext context) : Repository<Servico>(context), IServicoRepository
{
    public async Task<IReadOnlyList<Servico>> ListarPorEstabelecimentoAsync(Guid estabelecimentoId, CancellationToken cancellationToken = default) =>
        await Context.Servicos
            .Where(s => s.EstabelecimentoId == estabelecimentoId)
            .OrderBy(s => s.Nome)
            .ToListAsync(cancellationToken);
}
