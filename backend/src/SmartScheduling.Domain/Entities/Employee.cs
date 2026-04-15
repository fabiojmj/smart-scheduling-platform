using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Entities;

public class Employee : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public Guid EstablishmentId { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<WorkingHours> _workingHours = [];
    private readonly List<Service> _services = [];
    private readonly List<Appointment> _appointments = [];

    public IReadOnlyCollection<WorkingHours> WorkingHours => _workingHours.AsReadOnly();
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    private Employee() { Name = default!; Email = default!; }

    public static Employee Create(string name, string email, Guid establishmentId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Nome do funcionario e obrigatorio.");
        return new Employee { Name = name.Trim(), Email = email.Trim().ToLower(), EstablishmentId = establishmentId, IsActive = true };
    }

    public void AddWorkingHours(WorkingHours wh)
    {
        if (_workingHours.Any(w => w.DayOfWeek == wh.DayOfWeek)) throw new DomainException($"Horario para {wh.DayOfWeek} ja configurado.");
        _workingHours.Add(wh);
    }

    public void AssignService(Service service)
    {
        if (_services.Any(s => s.Id == service.Id)) throw new DomainException($"Servico ja atribuido.");
        _services.Add(service);
    }

    public bool IsAvailableAt(TimeSlot slot) =>
        _workingHours.Any(w => w.IsAvailableOn(slot.Start))
        && !_appointments.Any(a => a.IsActive() && a.TimeSlot.OverlapsWith(slot));

    public bool CanPerform(Service service) => _services.Any(s => s.Id == service.Id);
    public void Deactivate() { IsActive = false; MarkAsUpdated(); }
}
