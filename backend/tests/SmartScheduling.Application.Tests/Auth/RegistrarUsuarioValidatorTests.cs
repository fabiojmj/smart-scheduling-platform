using FluentAssertions;
using SmartScheduling.Application.Auth.Commands.RegistrarUsuario;

namespace SmartScheduling.Application.Tests.Auth;

public class RegistrarUsuarioValidatorTests
{
    private readonly RegistrarUsuarioValidator _validator = new();

    [Fact]
    public void Validate_DadosValidos_Passa()
    {
        var result = _validator.Validate(new RegistrarUsuarioCommand("João", "joao@test.com", "senha1234"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_NomeVazio_Falha(string nome)
    {
        var result = _validator.Validate(new RegistrarUsuarioCommand(nome, "joao@test.com", "senha1234"));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_NomeMuitoLongo_Falha()
    {
        var result = _validator.Validate(new RegistrarUsuarioCommand(new string('a', 101), "joao@test.com", "senha1234"));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("nao-e-email")]
    public void Validate_EmailInvalido_Falha(string email)
    {
        var result = _validator.Validate(new RegistrarUsuarioCommand("João", email, "senha1234"));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("curta")]
    public void Validate_SenhaInvalida_Falha(string senha)
    {
        var result = _validator.Validate(new RegistrarUsuarioCommand("João", "joao@test.com", senha));
        result.IsValid.Should().BeFalse();
    }
}
