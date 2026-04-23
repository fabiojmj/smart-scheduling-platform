using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface IRecurringScheduleRepository : IRepository<RecurringSchedule>
{
    Task<IReadOnlyList<RecurringSchedule>> GetByClientAsync(
        Guid clientId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecurringSchedule>> GetByEstablishmentAsync(
        Guid establishmentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecurringSchedule>> GetActiveAsync(
        Guid establishmentId,
        CancellationToken cancellationToken = default);
}
