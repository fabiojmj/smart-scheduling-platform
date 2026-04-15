using Microsoft.EntityFrameworkCore;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Persistence.Repositories;

public class AppointmentRepository(AppDbContext context) : Repository<Appointment>(context), IAppointmentRepository
{
    public async Task<IReadOnlyList<Appointment>> GetByEmployeeAsync(Guid employeeId, DateOnly date, CancellationToken cancellationToken = default) =>
        await Context.Appointments.Include(a => a.Client).Include(a => a.Service)
            .Where(a => a.EmployeeId == employeeId && a.TimeSlot.Start.Date == date.ToDateTime(TimeOnly.MinValue).Date)
            .OrderBy(a => a.TimeSlot.Start).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Appointment>> GetByClientAsync(Guid clientId, CancellationToken cancellationToken = default) =>
        await Context.Appointments.Include(a => a.Employee).Include(a => a.Service)
            .Where(a => a.ClientId == clientId).OrderByDescending(a => a.TimeSlot.Start).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Appointment>> GetByEstablishmentAsync(Guid establishmentId, DateOnly date, CancellationToken cancellationToken = default) =>
        await Context.Appointments.Include(a => a.Client).Include(a => a.Employee).Include(a => a.Service)
            .Where(a => a.EstablishmentId == establishmentId && a.TimeSlot.Start.Date == date.ToDateTime(TimeOnly.MinValue).Date)
            .OrderBy(a => a.TimeSlot.Start).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Appointment>> GetUpcomingAsync(TimeSpan within, CancellationToken cancellationToken = default)
    {
        var until = DateTime.UtcNow.Add(within);
        return await Context.Appointments.Include(a => a.Client).Include(a => a.Employee).Include(a => a.Service)
            .Where(a => a.Status == AppointmentStatus.Confirmed && a.TimeSlot.Start >= DateTime.UtcNow && a.TimeSlot.Start <= until)
            .ToListAsync(cancellationToken);
    }
}
