using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Entities;

public class Estabelecimento : Entity
{
    public string Nome { get; private set; }
    public string WhatsAppPhoneNumberId { get; private set; }
    public string ProprietarioId { get; private set; }
    public bool Ativo { get; private set; }
    public Guid? FuncionarioIdPrimeiroAtendimento { get; private set; }

    private readonly List<Funcionario> _funcionarios = [];
    private readonly List<Servico> _servicos = [];
    public IReadOnlyCollection<Funcionario> Funcionarios => _funcionarios.AsReadOnly();
    public IReadOnlyCollection<Servico> Servicos => _servicos.AsReadOnly();

    private Estabelecimento() { Nome = default!; WhatsAppPhoneNumberId = default!; ProprietarioId = default!; }

    public static Estabelecimento Criar(string nome, string whatsAppPhoneNumberId, string proprietarioId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome do estabelecimento e obrigatorio.");
        return new Estabelecimento { Nome = nome.Trim(), WhatsAppPhoneNumberId = whatsAppPhoneNumberId, ProprietarioId = proprietarioId, Ativo = true };
    }

    public void DefinirFuncionarioPrimeiroAtendimento(Guid? funcionarioId)
    {
        FuncionarioIdPrimeiroAtendimento = funcionarioId;
        MarkAsUpdated();
    }

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new DomainException("Nome e obrigatorio.");
        Nome = nome.Trim(); MarkAsUpdated();
    }

    public void Desativar() { Ativo = false; MarkAsUpdated(); }

    public void AdicionarFuncionario(Funcionario funcionario)
    {
        if (_funcionarios.Any(f => f.Id == funcionario.Id)) throw new DomainException("Funcionario ja cadastrado.");
        _funcionarios.Add(funcionario);
    }

    public void AdicionarServico(Servico servico)
    {
        if (_servicos.Any(s => s.Nome == servico.Nome)) throw new DomainException($"Servico '{servico.Nome}' ja existe.");
        _servicos.Add(servico);
    }
}
