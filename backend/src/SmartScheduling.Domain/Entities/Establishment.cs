using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Entities;

public class Establishment : Entity
{
    public string Name { get; private set; }
    public string WhatsAppPhoneNumberId { get; private set; }
    public string OwnerId { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Employee> _employees = [];
    private readonly List<Service> _services = [];
    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();

    private Establishment() { Name = default!; WhatsAppPhoneNumberId = default!; OwnerId = default!; }

    public static Establishment Create(string name, string whatsAppPhoneNumberId, string ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do estabelecimento e obrigatorio.");
        return new Establishment { Name = name.Trim(), WhatsAppPhoneNumberId = whatsAppPhoneNumberId, OwnerId = ownerId, IsActive = true };
    }

    public void UpdateName(string name) { if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Nome e obrigatorio."); Name = name.Trim(); MarkAsUpdated(); }
    public void Deactivate() { IsActive = false; MarkAsUpdated(); }

    public void AddEmployee(Employee employee)
    {
        if (_employees.Any(e => e.Id == employee.Id)) throw new DomainException("Funcionario ja cadastrado.");
        _employees.Add(employee);
    }

    public void AddService(Service service)
    {
        if (_services.Any(s => s.Name == service.Name)) throw new DomainException($"Servico '{service.Name}' ja existe.");
        _services.Add(service);
    }
}
