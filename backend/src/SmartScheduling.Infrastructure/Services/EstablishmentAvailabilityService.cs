using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Services;

public class EstablishmentAvailabilityService(
    IEstablishmentWorkingHoursRepository workingHoursRepository,
    IEstablishmentBlockRepository blockRepository) : IEstablishmentAvailabilityService
{
    public async Task<bool> EstaDisponivelAsync(Guid estabelecimentoId, DateTime dataHora, CancellationToken cancellationToken = default)
    {
        var bloqueio = await blockRepository.ObterBloqueioAtivoAsync(estabelecimentoId, dataHora, cancellationToken);
        if (bloqueio is not null)
            return false;

        var horario = await workingHoursRepository.ObterPorDiaAsync(estabelecimentoId, dataHora.DayOfWeek, cancellationToken);
        if (horario is null)
            return false;

        return horario.EstaAberto(TimeOnly.FromDateTime(dataHora));
    }
}
