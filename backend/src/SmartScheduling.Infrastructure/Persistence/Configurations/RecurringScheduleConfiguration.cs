using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Infrastructure.Persistence.Configurations;

public class RecurringScheduleConfiguration : IEntityTypeConfiguration<RecurringSchedule>
{
    public void Configure(EntityTypeBuilder<RecurringSchedule> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Frequency)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(r => r.Interval)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(r => r.DaysOfWeek)
            .HasConversion(
                v => string.Join(',', v.Select(d => (int)d)),
                v => v == "" ? [] : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(int.Parse)
                                     .Select(i => (DayOfWeek)i)
                                     .ToArray())
            .HasColumnType("varchar(20)");

        builder.Property(r => r.StartTime)
            .HasColumnType("time")
            .IsRequired();

        builder.Property(r => r.StartsOn)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(r => r.EndsOn)
            .HasColumnType("date");

        builder.Property(r => r.IsActive)
            .IsRequired();

        builder.HasIndex(r => new { r.EstablishmentId, r.IsActive });
        builder.HasIndex(r => r.ClientId);

        builder.HasOne<Client>()
            .WithMany()
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(r => r.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Service>()
            .WithMany()
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(r => r.Appointments);
    }
}
