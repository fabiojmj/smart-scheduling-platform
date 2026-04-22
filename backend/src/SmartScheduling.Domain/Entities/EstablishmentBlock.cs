using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Entities;

public class EstablishmentBlock : Entity
{
    public Guid EstabelecimentoId { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime DataFim { get; private set; }
    public string? Motivo { get; private set; }

    public Estabelecimento Estabelecimento { get; private set; } = default!;

    private EstablishmentBlock() { }

    public static EstablishmentBlock Criar(Guid estabelecimentoId, DateTime dataInicio, DateTime dataFim, string? motivo = null)
    {
        if (dataFim <= dataInicio)
            throw new DomainException("Data de fim do bloqueio deve ser apos o inicio.");

        return new EstablishmentBlock
        {
            EstabelecimentoId = estabelecimentoId,
            DataInicio = dataInicio.ToUniversalTime(),
            DataFim = dataFim.ToUniversalTime(),
            Motivo = motivo?.Trim()
        };
    }

    public bool CobreaData(DateTime data) => data >= DataInicio && data <= DataFim;
}
