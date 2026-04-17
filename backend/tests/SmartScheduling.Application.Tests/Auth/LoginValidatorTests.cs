using FluentAssertions;
using SmartScheduling.Application.Auth.Commands.Login;

namespace SmartScheduling.Application.Tests.Auth;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator = new();

    [Fact]
    public void Validate_DadosValidos_Passa()
    {
        var result = _validator.Validate(new LoginCommand("user@test.com", "senha123"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "senha123")]
    [InlineData("nao-e-email", "senha123")]
    public void Validate_EmailInvalido_Falha(string email, string senha)
    {
        var result = _validator.Validate(new LoginCommand(email, senha));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_SenhaVazia_Falha()
    {
        var result = _validator.Validate(new LoginCommand("user@test.com", ""));
        result.IsValid.Should().BeFalse();
    }
}
