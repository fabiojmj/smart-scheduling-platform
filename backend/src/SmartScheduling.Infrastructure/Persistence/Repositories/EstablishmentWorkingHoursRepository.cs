using Microsoft.EntityFrameworkCore;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Persistence.Repositories;

public class EstablishmentWorkingHoursRepository(AppDbContext context)
    : Repository<EstablishmentWorkingHours>(context), IEstablishmentWorkingHoursRepository
{
    public async Task<IReadOnlyList<EstablishmentWorkingHours>> ListarPorEstabelecimentoAsync(
        Guid estabelecimentoId, CancellationToken cancellationToken = default) =>
        await Context.HorariosEstabelecimento
            .Where(h => h.EstabelecimentoId == estabelecimentoId && h.Ativo)
            .OrderBy(h => h.DiaSemana)
            .ToListAsync(cancellationToken);

    public async Task<EstablishmentWorkingHours?> ObterPorDiaAsync(
        Guid estabelecimentoId, DayOfWeek diaSemana, CancellationToken cancellationToken = default) =>
        await Context.HorariosEstabelecimento
            .FirstOrDefaultAsync(h => h.EstabelecimentoId == estabelecimentoId && h.DiaSemana == diaSemana && h.Ativo, cancellationToken);
}
