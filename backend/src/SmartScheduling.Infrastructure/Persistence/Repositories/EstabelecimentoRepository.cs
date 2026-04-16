using Microsoft.EntityFrameworkCore;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Persistence.Repositories;

public class EstabelecimentoRepository(AppDbContext context) : Repository<Estabelecimento>(context), IEstabelecimentoRepository
{
    public async Task<Estabelecimento?> ObterPorProprietarioAsync(string proprietarioId, CancellationToken cancellationToken = default) =>
        await Context.Estabelecimentos
            .FirstOrDefaultAsync(e => e.ProprietarioId == proprietarioId && e.Ativo, cancellationToken);
}
