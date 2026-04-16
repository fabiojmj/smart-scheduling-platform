using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Entities;

public class Cliente : Entity
{
    public string Nome { get; private set; }
    public PhoneNumber Telefone { get; private set; }
    public string? Email { get; private set; }
    public Guid EstabelecimentoId { get; private set; }

    private readonly List<Agendamento> _agendamentos = [];
    public IReadOnlyCollection<Agendamento> Agendamentos => _agendamentos.AsReadOnly();

    private Cliente() { Nome = default!; Telefone = default!; }

    public static Cliente Criar(string nome, string telefone, Guid estabelecimentoId, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new DomainException("Nome do cliente e obrigatorio.");
        return new Cliente
        {
            Nome = nome.Trim(), Telefone = PhoneNumber.Create(telefone),
            EstabelecimentoId = estabelecimentoId, Email = email?.Trim().ToLower()
        };
    }

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new DomainException("Nome obrigatorio.");
        Nome = nome.Trim(); MarkAsUpdated();
    }
}
