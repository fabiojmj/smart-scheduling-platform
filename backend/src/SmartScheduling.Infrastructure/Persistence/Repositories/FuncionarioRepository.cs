using Microsoft.EntityFrameworkCore;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Persistence.Repositories;

public class FuncionarioRepository(AppDbContext context) : Repository<Funcionario>(context), IFuncionarioRepository
{
    public async Task<IReadOnlyList<Funcionario>> ListarPorEstabelecimentoAsync(Guid estabelecimentoId, CancellationToken cancellationToken = default) =>
        await Context.Funcionarios
            .Where(f => f.EstabelecimentoId == estabelecimentoId)
            .OrderBy(f => f.Nome)
            .ToListAsync(cancellationToken);

    public async Task<Funcionario?> ObterComDetalhesAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Funcionarios
            .Include(f => f.Servicos)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
}
