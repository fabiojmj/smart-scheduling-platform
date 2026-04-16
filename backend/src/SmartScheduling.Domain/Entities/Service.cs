using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Entities;

public class Servico : Entity
{
    public string Nome { get; private set; }
    public string? Descricao { get; private set; }
    public int DuracaoMinutos { get; private set; }
    public decimal Preco { get; private set; }
    public Guid EstabelecimentoId { get; private set; }
    public bool Ativo { get; private set; }

    private Servico() { Nome = default!; }

    public static Servico Criar(string nome, int duracaoMinutos, decimal preco, Guid estabelecimentoId, string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new DomainException("Nome do servico e obrigatorio.");
        if (duracaoMinutos <= 0) throw new DomainException("Duracao deve ser maior que zero.");
        if (preco < 0) throw new DomainException("Preco nao pode ser negativo.");
        return new Servico
        {
            Nome = nome.Trim(), Descricao = descricao?.Trim(),
            DuracaoMinutos = duracaoMinutos, Preco = preco,
            EstabelecimentoId = estabelecimentoId, Ativo = true
        };
    }

    public void AtualizarPreco(decimal preco) { if (preco < 0) throw new DomainException("Preco invalido."); Preco = preco; MarkAsUpdated(); }
    public void AtualizarDuracao(int minutos) { if (minutos <= 0) throw new DomainException("Duracao invalida."); DuracaoMinutos = minutos; MarkAsUpdated(); }
    public void Desativar() { Ativo = false; MarkAsUpdated(); }
}
