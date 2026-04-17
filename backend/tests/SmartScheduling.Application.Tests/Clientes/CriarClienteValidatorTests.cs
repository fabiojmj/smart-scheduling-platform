using FluentAssertions;
using SmartScheduling.Application.Clientes.Commands.CriarCliente;

namespace SmartScheduling.Application.Tests.Clientes;

public class CriarClienteValidatorTests
{
    private readonly CriarClienteValidator _validator = new();

    [Fact]
    public void Validate_DadosValidos_Passa()
    {
        var result = _validator.Validate(new CriarClienteCommand("João", "11987654321", Guid.NewGuid()));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComEmail_Passa()
    {
        var result = _validator.Validate(new CriarClienteCommand("João", "11987654321", Guid.NewGuid(), "joao@test.com"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_NomeVazio_Falha(string nome)
    {
        var result = _validator.Validate(new CriarClienteCommand(nome, "11987654321", Guid.NewGuid()));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_NomeMuitoLongo_Falha()
    {
        var result = _validator.Validate(new CriarClienteCommand(new string('a', 201), "11987654321", Guid.NewGuid()));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("abc")]
    public void Validate_TelefoneInvalido_Falha(string telefone)
    {
        var result = _validator.Validate(new CriarClienteCommand("João", telefone, Guid.NewGuid()));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EstabelecimentoIdVazio_Falha()
    {
        var result = _validator.Validate(new CriarClienteCommand("João", "11987654321", Guid.Empty));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmailInvalido_Falha()
    {
        var result = _validator.Validate(new CriarClienteCommand("João", "11987654321", Guid.NewGuid(), "nao-e-email"));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmailMuitoLongo_Falha()
    {
        var email = new string('a', 195) + "@b.com";
        var result = _validator.Validate(new CriarClienteCommand("João", "11987654321", Guid.NewGuid(), email));
        result.IsValid.Should().BeFalse();
    }
}
