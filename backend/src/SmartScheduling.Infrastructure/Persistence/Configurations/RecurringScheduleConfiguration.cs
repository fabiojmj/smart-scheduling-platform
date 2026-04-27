using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        var daysComparer = new ValueComparer<DayOfWeek[]>(
            (a, b) => a != null && b != null && a.SequenceEqual(b),
            v => v.Aggregate(0, (a, d) => HashCode.Combine(a, d.GetHashCode())),
            v => v.ToArray());

        builder.Property(r => r.DaysOfWeek)
            .HasConversion(
                v => string.Join(',', v.Select(d => (int)d)),
                v => v == "" ? Array.Empty<DayOfWeek>() : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(int.Parse)
                                     .Select(i => (DayOfWeek)i)
                                     .ToArray())
            .HasColumnType("varchar(20)")
            .Metadata.SetValueComparer(daysComparer);

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

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Funcionario>()
            .WithMany()
            .HasForeignKey(r => r.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Servico>()
            .WithMany()
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(r => r.Appointments);
    }
}
