using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.Events;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Entities;

public class Agendamento : Entity
{
    public Guid ClienteId { get; private set; }
    public Guid FuncionarioId { get; private set; }
    public Guid ServicoId { get; private set; }
    public Guid EstabelecimentoId { get; private set; }
    public TimeSlot Horario { get; private set; }
    public StatusAgendamento Status { get; private set; }
    public string? Observacoes { get; private set; }
    public string? MotivoCancelamento { get; private set; }

    public Cliente Cliente { get; private set; } = default!;
    public Funcionario Funcionario { get; private set; } = default!;
    public Servico Servico { get; private set; } = default!;

    private Agendamento() { Horario = default!; }

    public static Agendamento Agendar(Cliente cliente, Funcionario funcionario, Servico servico, TimeSlot horario, string? observacoes = null)
    {
        if (!funcionario.EstaDisponivel(horario))
            throw new SchedulingConflictException(funcionario.Nome, horario.Start, horario.End);
        if (!funcionario.PodeExecutar(servico))
            throw new DomainException("Funcionario nao executa este servico.");

        var a = new Agendamento
        {
            ClienteId = cliente.Id, FuncionarioId = funcionario.Id, ServicoId = servico.Id,
            EstabelecimentoId = servico.EstabelecimentoId, Horario = horario,
            Status = StatusAgendamento.Pendente, Observacoes = observacoes
        };
        a.AddDomainEvent(new AgendamentoCriadoEvent(a.Id, cliente.Id, funcionario.Id, horario.Start, servico.Nome));
        return a;
    }

    public void Confirmar()
    {
        if (Status != StatusAgendamento.Pendente) throw new DomainException("Apenas pendentes podem ser confirmados.");
        Status = StatusAgendamento.Confirmado; MarkAsUpdated();
    }

    public void Concluir()
    {
        if (Status != StatusAgendamento.Confirmado) throw new DomainException("Apenas confirmados podem ser concluidos.");
        Status = StatusAgendamento.Concluido; MarkAsUpdated();
    }

    public void Cancelar(string motivo)
    {
        if (Status is StatusAgendamento.Concluido or StatusAgendamento.Cancelado)
            throw new DomainException("Agendamento ja finalizado.");
        if (string.IsNullOrWhiteSpace(motivo)) throw new DomainException("Motivo obrigatorio.");
        Status = StatusAgendamento.Cancelado; MotivoCancelamento = motivo; MarkAsUpdated();
        AddDomainEvent(new AgendamentoCanceladoEvent(Id, ClienteId, motivo, DateTime.UtcNow));
    }

    public void MarcarNaoCompareceu()
    {
        if (Status != StatusAgendamento.Confirmado) throw new DomainException("Apenas confirmados podem ser marcados como nao compareceu.");
        Status = StatusAgendamento.NaoCompareceu; MarkAsUpdated();
    }

    public bool EstaAtivo() => Status is StatusAgendamento.Pendente or StatusAgendamento.Confirmado;
}
