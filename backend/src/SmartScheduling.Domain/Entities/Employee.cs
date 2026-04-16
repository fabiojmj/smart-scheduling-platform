using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Entities;

public class Funcionario : Entity
{
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public Guid EstabelecimentoId { get; private set; }
    public bool Ativo { get; private set; }

    private readonly List<WorkingHours> _horarios = [];
    private readonly List<Servico> _servicos = [];
    private readonly List<Agendamento> _agendamentos = [];

    public IReadOnlyCollection<WorkingHours> Horarios => _horarios.AsReadOnly();
    public IReadOnlyCollection<Servico> Servicos => _servicos.AsReadOnly();
    public IReadOnlyCollection<Agendamento> Agendamentos => _agendamentos.AsReadOnly();

    private Funcionario() { Nome = default!; Email = default!; }

    public static Funcionario Criar(string nome, string email, Guid estabelecimentoId)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new DomainException("Nome do funcionario e obrigatorio.");
        return new Funcionario { Nome = nome.Trim(), Email = email.Trim().ToLower(), EstabelecimentoId = estabelecimentoId, Ativo = true };
    }

    public void AdicionarHorario(WorkingHours horario)
    {
        if (_horarios.Any(h => h.DayOfWeek == horario.DayOfWeek))
            throw new DomainException($"Horario para {horario.DayOfWeek} ja configurado.");
        _horarios.Add(horario);
    }

    public void AtribuirServico(Servico servico)
    {
        if (_servicos.Any(s => s.Id == servico.Id)) throw new DomainException("Servico ja atribuido.");
        _servicos.Add(servico);
    }

    public bool EstaDisponivel(TimeSlot horario) =>
        _horarios.Any(h => h.IsAvailableOn(horario.Start))
        && !_agendamentos.Any(a => a.EstaAtivo() && a.Horario.OverlapsWith(horario));

    public bool PodeExecutar(Servico servico) => _servicos.Any(s => s.Id == servico.Id);
    public void Desativar() { Ativo = false; MarkAsUpdated(); }
}
