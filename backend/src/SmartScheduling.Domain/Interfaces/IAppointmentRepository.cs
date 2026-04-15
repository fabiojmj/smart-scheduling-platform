using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetByEmployeeAsync(Guid employeeId, DateOnly date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByClientAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByEstablishmentAsync(Guid establishmentId, DateOnly date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetUpcomingAsync(TimeSpan within, CancellationToken cancellationToken = default);
}
