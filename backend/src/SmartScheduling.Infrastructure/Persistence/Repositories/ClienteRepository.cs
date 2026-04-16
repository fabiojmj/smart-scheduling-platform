using Microsoft.EntityFrameworkCore;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Persistence.Repositories;

public class ClienteRepository(AppDbContext context) : Repository<Cliente>(context), IClienteRepository
{
    public async Task<IReadOnlyList<Cliente>> ListarPorEstabelecimentoAsync(Guid estabelecimentoId, CancellationToken cancellationToken = default) =>
        await Context.Clientes
            .Where(c => c.EstabelecimentoId == estabelecimentoId)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
}
