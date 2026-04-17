using FluentAssertions;
using Moq;
using SmartScheduling.Application.Funcionarios.Commands.CriarFuncionario;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Funcionarios;

public class CriarFuncionarioHandlerTests
{
    private readonly Mock<IFuncionarioRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly CriarFuncionarioHandler _handler;

    public CriarFuncionarioHandlerTests()
    {
        _handler = new CriarFuncionarioHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_DadosValidos_CriaFuncionarioERetornaId()
    {
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Funcionario>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(
            new CriarFuncionarioCommand("Ana Costa", "ana@test.com", Guid.NewGuid()),
            CancellationToken.None);

        result.Should().NotBeEmpty();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Funcionario>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class CriarFuncionarioValidatorTests
{
    private readonly CriarFuncionarioValidator _validator = new();

    [Fact]
    public void Validate_DadosValidos_Passa()
    {
        var result = _validator.Validate(new CriarFuncionarioCommand("Ana Costa", "ana@test.com", Guid.NewGuid()));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_NomeVazio_Falha(string nome)
    {
        var result = _validator.Validate(new CriarFuncionarioCommand(nome, "ana@test.com", Guid.NewGuid()));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_NomeMuitoLongo_Falha()
    {
        var result = _validator.Validate(new CriarFuncionarioCommand(new string('a', 201), "ana@test.com", Guid.NewGuid()));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("nao-e-email")]
    public void Validate_EmailInvalido_Falha(string email)
    {
        var result = _validator.Validate(new CriarFuncionarioCommand("Ana", email, Guid.NewGuid()));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmailMuitoLongo_Falha()
    {
        var email = new string('a', 195) + "@b.com";
        var result = _validator.Validate(new CriarFuncionarioCommand("Ana", email, Guid.NewGuid()));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EstabelecimentoIdVazio_Falha()
    {
        var result = _validator.Validate(new CriarFuncionarioCommand("Ana", "ana@test.com", Guid.Empty));
        result.IsValid.Should().BeFalse();
    }
}
