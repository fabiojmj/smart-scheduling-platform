using FluentAssertions;
using Moq;
using SmartScheduling.Application.Servicos.Commands.CriarServico;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Servicos;

public class CriarServicoHandlerTests
{
    private readonly Mock<IServicoRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly CriarServicoHandler _handler;

    public CriarServicoHandlerTests()
    {
        _handler = new CriarServicoHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_DadosValidos_CriaServicoERetornaId()
    {
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Servico>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(
            new CriarServicoCommand("Corte", 30, 50m, Guid.NewGuid(), "Descrição"),
            CancellationToken.None);

        result.Should().NotBeEmpty();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Servico>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SemDescricao_CriaServicoSemDescricao()
    {
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Servico>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(
            new CriarServicoCommand("Corte", 30, 50m, Guid.NewGuid()),
            CancellationToken.None);

        result.Should().NotBeEmpty();
    }
}

public class CriarServicoValidatorTests
{
    private readonly CriarServicoValidator _validator = new();

    [Fact]
    public void Validate_DadosValidos_Passa()
    {
        var result = _validator.Validate(new CriarServicoCommand("Corte", 30, 50m, Guid.NewGuid()));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComDescricao_Passa()
    {
        var result = _validator.Validate(new CriarServicoCommand("Corte", 30, 50m, Guid.NewGuid(), "desc"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_NomeVazio_Falha(string nome)
    {
        var result = _validator.Validate(new CriarServicoCommand(nome, 30, 50m, Guid.NewGuid()));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_NomeMuitoLongo_Falha()
    {
        var result = _validator.Validate(new CriarServicoCommand(new string('a', 201), 30, 50m, Guid.NewGuid()));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_DuracaoInvalida_Falha(int duracao)
    {
        var result = _validator.Validate(new CriarServicoCommand("Corte", duracao, 50m, Guid.NewGuid()));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_PrecoNegativo_Falha()
    {
        var result = _validator.Validate(new CriarServicoCommand("Corte", 30, -1m, Guid.NewGuid()));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EstabelecimentoIdVazio_Falha()
    {
        var result = _validator.Validate(new CriarServicoCommand("Corte", 30, 50m, Guid.Empty));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_DescricaoMuitoLonga_Falha()
    {
        var result = _validator.Validate(new CriarServicoCommand("Corte", 30, 50m, Guid.NewGuid(), new string('a', 1001)));
        result.IsValid.Should().BeFalse();
    }
}
