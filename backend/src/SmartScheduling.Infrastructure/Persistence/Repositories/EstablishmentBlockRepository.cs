using Microsoft.EntityFrameworkCore;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Persistence.Repositories;

public class EstablishmentBlockRepository(AppDbContext context)
    : Repository<EstablishmentBlock>(context), IEstablishmentBlockRepository
{
    public async Task<IReadOnlyList<EstablishmentBlock>> ListarPorEstabelecimentoAsync(
        Guid estabelecimentoId, CancellationToken cancellationToken = default) =>
        await Context.BloqueiosEstabelecimento
            .Where(b => b.EstabelecimentoId == estabelecimentoId)
            .OrderByDescending(b => b.DataInicio)
            .ToListAsync(cancellationToken);

    public async Task<EstablishmentBlock?> ObterBloqueioAtivoAsync(
        Guid estabelecimentoId, DateTime data, CancellationToken cancellationToken = default) =>
        await Context.BloqueiosEstabelecimento
            .FirstOrDefaultAsync(b => b.EstabelecimentoId == estabelecimentoId
                && b.DataInicio <= data && b.DataFim >= data, cancellationToken);
}
