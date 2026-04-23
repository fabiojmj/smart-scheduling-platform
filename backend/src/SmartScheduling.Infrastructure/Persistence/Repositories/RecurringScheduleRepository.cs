using Microsoft.EntityFrameworkCore;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Persistence.Repositories;

public class RecurringScheduleRepository(AppDbContext context)
    : Repository<RecurringSchedule>(context), IRecurringScheduleRepository
{
    public async Task<IReadOnlyList<RecurringSchedule>> GetByClientAsync(
        Guid clientId,
        CancellationToken cancellationToken = default) =>
        await Context.RecurringSchedules
            .Where(r => r.ClientId == clientId && r.IsActive)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RecurringSchedule>> GetByEstablishmentAsync(
        Guid establishmentId,
        CancellationToken cancellationToken = default) =>
        await Context.RecurringSchedules
            .Where(r => r.EstablishmentId == establishmentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RecurringSchedule>> GetActiveAsync(
        Guid establishmentId,
        CancellationToken cancellationToken = default) =>
        await Context.RecurringSchedules
            .Where(r => r.EstablishmentId == establishmentId && r.IsActive)
            .ToListAsync(cancellationToken);
}
