using FluentAssertions;
using Moq;
using SmartScheduling.Application.Estabelecimentos.Commands.CriarEstabelecimento;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Estabelecimentos;

public class CriarEstabelecimentoHandlerTests
{
    private readonly Mock<IEstabelecimentoRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly CriarEstabelecimentoHandler _handler;

    public CriarEstabelecimentoHandlerTests()
    {
        _handler = new CriarEstabelecimentoHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_DadosValidos_CriaEstabelecimentoERetornaId()
    {
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Estabelecimento>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(
            new CriarEstabelecimentoCommand("Barbearia Top", "whatsapp-id-123", "prop-id-abc"),
            CancellationToken.None);

        result.Should().NotBeEmpty();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Estabelecimento>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class CriarEstabelecimentoValidatorTests
{
    private readonly CriarEstabelecimentoValidator _validator = new();

    [Fact]
    public void Validate_DadosValidos_Passa()
    {
        var result = _validator.Validate(new CriarEstabelecimentoCommand("Barbearia", "wa-123", "prop-id"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_NomeVazio_Falha(string nome)
    {
        var result = _validator.Validate(new CriarEstabelecimentoCommand(nome, "wa-123", "prop-id"));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_NomeMuitoLongo_Falha()
    {
        var result = _validator.Validate(new CriarEstabelecimentoCommand(new string('a', 201), "wa-123", "prop-id"));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhatsAppIdVazio_Falha(string waId)
    {
        var result = _validator.Validate(new CriarEstabelecimentoCommand("Barbearia", waId, "prop-id"));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhatsAppIdMuitoLongo_Falha()
    {
        var result = _validator.Validate(new CriarEstabelecimentoCommand("Barbearia", new string('a', 51), "prop-id"));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ProprietarioIdVazio_Falha(string propId)
    {
        var result = _validator.Validate(new CriarEstabelecimentoCommand("Barbearia", "wa-123", propId));
        result.IsValid.Should().BeFalse();
    }
}
