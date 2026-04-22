namespace SmartScheduling.Domain.Interfaces;

public interface IEstablishmentAvailabilityService
{
    Task<bool> EstaDisponivelAsync(Guid estabelecimentoId, DateTime dataHora, CancellationToken cancellationToken = default);
}
