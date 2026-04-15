using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Entities;

public class Service : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal Price { get; private set; }
    public Guid EstablishmentId { get; private set; }
    public bool IsActive { get; private set; }

    private Service() { Name = default!; }

    public static Service Create(string name, int durationMinutes, decimal price, Guid establishmentId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Nome do servico e obrigatorio.");
        if (durationMinutes <= 0) throw new DomainException("Duracao deve ser maior que zero.");
        if (price < 0) throw new DomainException("Preco nao pode ser negativo.");
        return new Service { Name = name.Trim(), Description = description?.Trim(), DurationMinutes = durationMinutes, Price = price, EstablishmentId = establishmentId, IsActive = true };
    }

    public void UpdatePrice(decimal p) { if (p < 0) throw new DomainException("Preco invalido."); Price = p; MarkAsUpdated(); }
    public void UpdateDuration(int m) { if (m <= 0) throw new DomainException("Duracao invalida."); DurationMinutes = m; MarkAsUpdated(); }
    public void Deactivate() { IsActive = false; MarkAsUpdated(); }
}
