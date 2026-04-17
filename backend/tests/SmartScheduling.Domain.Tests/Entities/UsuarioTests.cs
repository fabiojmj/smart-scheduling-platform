using FluentAssertions;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Tests.Entities;

public class UsuarioTests
{
    [Fact]
    public void Criar_ComDadosValidos_CriaComSucesso()
    {
        var usuario = Usuario.Criar("João Silva", "joao@test.com", "hash123");

        usuario.Nome.Should().Be("João Silva");
        usuario.Email.Should().Be("joao@test.com");
        usuario.PasswordHash.Should().Be("hash123");
        usuario.Role.Should().Be(Role.Proprietario);
        usuario.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Criar_EmailConverteParaMinusculas()
    {
        var usuario = Usuario.Criar("João", "JOAO@TEST.COM", "hash");
        usuario.Email.Should().Be("joao@test.com");
    }

    [Fact]
    public void Criar_NomeTrimado()
    {
        var usuario = Usuario.Criar("  João  ", "joao@test.com", "hash");
        usuario.Nome.Should().Be("João");
    }

    [Fact]
    public void Criar_ComRoleAdmin_DefinaRoleAdmin()
    {
        var usuario = Usuario.Criar("João", "joao@test.com", "hash", Role.Admin);
        usuario.Role.Should().Be(Role.Admin);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComNomeVazio_LancaDomainException(string nome)
    {
        var act = () => Usuario.Criar(nome, "joao@test.com", "hash");
        act.Should().Throw<DomainException>().WithMessage("*obrigatório*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComEmailVazio_LancaDomainException(string email)
    {
        var act = () => Usuario.Criar("João", email, "hash");
        act.Should().Throw<DomainException>().WithMessage("*obrigatório*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComPasswordHashVazio_LancaDomainException(string hash)
    {
        var act = () => Usuario.Criar("João", "joao@test.com", hash);
        act.Should().Throw<DomainException>().WithMessage("*obrigatória*");
    }

    [Fact]
    public void AtualizarSenha_SubstituiHash()
    {
        var usuario = Usuario.Criar("João", "joao@test.com", "hash-antigo");
        usuario.AtualizarSenha("hash-novo");
        usuario.PasswordHash.Should().Be("hash-novo");
    }
}
