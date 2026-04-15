using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Entities;

public class Client : Entity
{
    public string Name { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public Guid EstablishmentId { get; private set; }

    private readonly List<Appointment> _appointments = [];
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    private Client() { Name = default!; PhoneNumber = default!; }

    public static Client Create(string name, string phoneNumber, Guid establishmentId, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Nome do cliente e obrigatorio.");
        return new Client { Name = name.Trim(), PhoneNumber = PhoneNumber.Create(phoneNumber), EstablishmentId = establishmentId, Email = email?.Trim().ToLower() };
    }

    public void UpdateName(string name) { if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Nome obrigatorio."); Name = name.Trim(); MarkAsUpdated(); }
}
