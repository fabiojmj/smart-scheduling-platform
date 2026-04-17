using FluentAssertions;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Tests.Entities;

public class ClienteTests
{
    [Fact]
    public void Criar_ComDadosValidos_CriaComSucesso()
    {
        var id = Guid.NewGuid();
        var c = Cliente.Criar("João Silva", "11987654321", id, "joao@test.com");
        c.Nome.Should().Be("João Silva");
        c.Telefone.Value.Should().Be("11987654321");
        c.EstabelecimentoId.Should().Be(id);
        c.Email.Should().Be("joao@test.com");
        c.Agendamentos.Should().BeEmpty();
    }

    [Fact]
    public void Criar_SemEmail_EmailNulo()
    {
        var c = Cliente.Criar("João", "11987654321", Guid.NewGuid());
        c.Email.Should().BeNull();
    }

    [Fact]
    public void Criar_EmailEmMaiusculas_ConverteParaMinusculas()
    {
        var c = Cliente.Criar("João", "11987654321", Guid.NewGuid(), "JOAO@TEST.COM");
        c.Email.Should().Be("joao@test.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComNomeVazio_LancaDomainException(string nome)
    {
        var act = () => Cliente.Criar(nome, "11987654321", Guid.NewGuid());
        act.Should().Throw<DomainException>().WithMessage("*obrigatorio*");
    }

    [Fact]
    public void Criar_ComTelefoneInvalido_LancaArgumentException()
    {
        var act = () => Cliente.Criar("João", "123", Guid.NewGuid());
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AtualizarNome_ComNomeValido_AtualizaComSucesso()
    {
        var c = Cliente.Criar("João Velho", "11987654321", Guid.NewGuid());
        c.AtualizarNome("João Novo");
        c.Nome.Should().Be("João Novo");
        c.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AtualizarNome_ComNomeVazio_LancaDomainException(string nome)
    {
        var c = Cliente.Criar("João", "11987654321", Guid.NewGuid());
        var act = () => c.AtualizarNome(nome);
        act.Should().Throw<DomainException>().WithMessage("*obrigatorio*");
    }
}
