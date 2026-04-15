using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.Events;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Entities;

public class Appointment : Entity
{
    public Guid ClientId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Guid ServiceId { get; private set; }
    public Guid EstablishmentId { get; private set; }
    public TimeSlot TimeSlot { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }

    public Client Client { get; private set; } = default!;
    public Employee Employee { get; private set; } = default!;
    public Service Service { get; private set; } = default!;

    private Appointment() { TimeSlot = default!; }

    public static Appointment Schedule(Client client, Employee employee, Service service, TimeSlot slot, string? notes = null)
    {
        if (!employee.IsAvailableAt(slot))
            throw new SchedulingConflictException(employee.Name, slot.Start, slot.End);
        if (!employee.CanPerform(service))
            throw new DomainException($"Funcionario nao executa este servico.");

        var a = new Appointment
        {
            ClientId = client.Id, EmployeeId = employee.Id, ServiceId = service.Id,
            EstablishmentId = service.EstablishmentId, TimeSlot = slot,
            Status = AppointmentStatus.Pending, Notes = notes
        };
        a.AddDomainEvent(new AppointmentScheduledEvent(a.Id, client.Id, employee.Id, slot.Start, service.Name));
        return a;
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Pending) throw new DomainException("Apenas pendentes podem ser confirmados.");
        Status = AppointmentStatus.Confirmed; MarkAsUpdated();
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.Confirmed) throw new DomainException("Apenas confirmados podem ser concluidos.");
        Status = AppointmentStatus.Completed; MarkAsUpdated();
    }

    public void Cancel(string reason)
    {
        if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            throw new DomainException("Agendamento ja finalizado.");
        if (string.IsNullOrWhiteSpace(reason)) throw new DomainException("Motivo obrigatorio.");
        Status = AppointmentStatus.Cancelled; CancellationReason = reason; MarkAsUpdated();
        AddDomainEvent(new AppointmentCancelledEvent(Id, ClientId, reason, DateTime.UtcNow));
    }

    public void MarkAsNoShow()
    {
        if (Status != AppointmentStatus.Confirmed) throw new DomainException("Apenas confirmados podem ser NoShow.");
        Status = AppointmentStatus.NoShow; MarkAsUpdated();
    }

    public bool IsActive() => Status is AppointmentStatus.Pending or AppointmentStatus.Confirmed;
}
