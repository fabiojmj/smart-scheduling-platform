using FluentAssertions;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("11987654321")]
    [InlineData("1198765432")]
    [InlineData("123456789012345")]
    public void Create_ComNumeroValido_CriaComSucesso(string number)
    {
        var phone = PhoneNumber.Create(number);
        phone.Value.Should().Be(new string(number.Where(char.IsDigit).ToArray()));
    }

    [Fact]
    public void Create_ComNumerosComMascara_ExtraiDigitos()
    {
        var phone = PhoneNumber.Create("(11) 98765-4321");
        phone.Value.Should().Be("11987654321");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ComVazio_LancaArgumentException(string value)
    {
        var act = () => PhoneNumber.Create(value);
        act.Should().Throw<ArgumentException>().WithMessage("*vazio*");
    }

    [Theory]
    [InlineData("123456789")]       // 9 digits
    [InlineData("1234567890123456")] // 16 digits
    public void Create_ComTamanhoInvalido_LancaArgumentException(string value)
    {
        var act = () => PhoneNumber.Create(value);
        act.Should().Throw<ArgumentException>().WithMessage("*invalido*");
    }

    [Fact]
    public void Equals_ComMesmoValor_RetornaVerdadeiro()
    {
        var p1 = PhoneNumber.Create("11987654321");
        var p2 = PhoneNumber.Create("11987654321");
        p1.Should().Be(p2);
        p1.GetHashCode().Should().Be(p2.GetHashCode());
    }

    [Fact]
    public void Equals_ComValorDiferente_RetornaFalso()
    {
        var p1 = PhoneNumber.Create("11987654321");
        var p2 = PhoneNumber.Create("11987654322");
        p1.Should().NotBe(p2);
    }

    [Fact]
    public void Equals_ComTiposDiferentes_RetornaFalso()
    {
        var phone = PhoneNumber.Create("11987654321");
        phone.Equals("11987654321").Should().BeFalse();
    }

    [Fact]
    public void ToString_RetornaValor()
    {
        var phone = PhoneNumber.Create("11987654321");
        phone.ToString().Should().Be("11987654321");
    }
}
