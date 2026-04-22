using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Entities;

public class EstablishmentWorkingHours : Entity
{
    public Guid EstabelecimentoId { get; private set; }
    public DayOfWeek DiaSemana { get; private set; }
    public TimeOnly HoraInicio { get; private set; }
    public TimeOnly HoraFim { get; private set; }
    public bool Ativo { get; private set; }

    public Estabelecimento Estabelecimento { get; private set; } = default!;

    private EstablishmentWorkingHours() { }

    public static EstablishmentWorkingHours Criar(Guid estabelecimentoId, DayOfWeek diaSemana, TimeOnly horaInicio, TimeOnly horaFim)
    {
        if (horaFim <= horaInicio)
            throw new DomainException("Horario de encerramento deve ser apos o inicio.");

        return new EstablishmentWorkingHours
        {
            EstabelecimentoId = estabelecimentoId,
            DiaSemana = diaSemana,
            HoraInicio = horaInicio,
            HoraFim = horaFim,
            Ativo = true
        };
    }

    public void Atualizar(TimeOnly horaInicio, TimeOnly horaFim)
    {
        if (horaFim <= horaInicio)
            throw new DomainException("Horario de encerramento deve ser apos o inicio.");

        HoraInicio = horaInicio;
        HoraFim = horaFim;
        MarkAsUpdated();
    }

    public void Desativar() { Ativo = false; MarkAsUpdated(); }

    public bool EstaAberto(TimeOnly horario) => Ativo && horario >= HoraInicio && horario <= HoraFim;
}
