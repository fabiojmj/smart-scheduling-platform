namespace SmartScheduling.Domain.Events;

public sealed record AgendamentoCriadoEvent(
    Guid AgendamentoId,
    Guid ClienteId,
    Guid FuncionarioId,
    DateTime AgendadoEm,
    string NomeServico
);
