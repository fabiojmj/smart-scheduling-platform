namespace SmartScheduling.Domain.Events;

public sealed record AgendamentoCanceladoEvent(
    Guid AgendamentoId,
    Guid ClienteId,
    string Motivo,
    DateTime CanceladoEm
);
