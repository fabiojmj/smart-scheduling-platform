using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface IEstablishmentWorkingHoursRepository : IRepository<EstablishmentWorkingHours>
{
    Task<IReadOnlyList<EstablishmentWorkingHours>> ListarPorEstabelecimentoAsync(Guid estabelecimentoId, CancellationToken cancellationToken = default);
    Task<EstablishmentWorkingHours?> ObterPorDiaAsync(Guid estabelecimentoId, DayOfWeek diaSemana, CancellationToken cancellationToken = default);
}
