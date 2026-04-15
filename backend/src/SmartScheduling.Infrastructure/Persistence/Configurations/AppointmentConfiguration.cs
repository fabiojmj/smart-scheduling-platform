using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);
        builder.OwnsOne(a => a.TimeSlot, ts =>
        {
            ts.Property(t => t.Start).HasColumnName("StartAt").IsRequired();
            ts.Property(t => t.End).HasColumnName("EndAt").IsRequired();
        });
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.CancellationReason).HasMaxLength(500);
        builder.Property(a => a.Notes).HasMaxLength(1000);
        builder.HasOne(a => a.Client).WithMany(c => c.Appointments).HasForeignKey(a => a.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Employee).WithMany(e => e.Appointments).HasForeignKey(a => a.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Service).WithMany().HasForeignKey(a => a.ServiceId).OnDelete(DeleteBehavior.Restrict);
        builder.Ignore(a => a.DomainEvents);
    }
}
