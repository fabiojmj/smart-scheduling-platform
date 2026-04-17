using FluentAssertions;
using Moq;
using SmartScheduling.Application.Estabelecimentos.Commands.AtualizarEstabelecimento;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Estabelecimentos;

public class AtualizarNomeEstabelecimentoHandlerTests
{
    private readonly Mock<IEstabelecimentoRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly AtualizarNomeEstabelecimentoHandler _handler;

    public AtualizarNomeEstabelecimentoHandlerTests()
    {
        _handler = new AtualizarNomeEstabelecimentoHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_EstabelecimentoEncontrado_AtualizaNome()
    {
        var est = Estabelecimento.Criar("Barbearia", "wa-123", "prop-id");
        _repoMock.Setup(r => r.GetByIdAsync(est.Id, It.IsAny<CancellationToken>())).ReturnsAsync(est);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(new AtualizarNomeEstabelecimentoCommand(est.Id, "Novo Nome"), CancellationToken.None);

        est.Nome.Should().Be("Novo Nome");
        _repoMock.Verify(r => r.Update(est), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EstabelecimentoNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Estabelecimento?)null);

        var act = () => _handler.Handle(
            new AtualizarNomeEstabelecimentoCommand(Guid.NewGuid(), "Nome"),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Estabelecimento*");
    }
}

public class DesativarEstabelecimentoHandlerTests
{
    private readonly Mock<IEstabelecimentoRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly DesativarEstabelecimentoHandler _handler;

    public DesativarEstabelecimentoHandlerTests()
    {
        _handler = new DesativarEstabelecimentoHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_EstabelecimentoEncontrado_Desativa()
    {
        var est = Estabelecimento.Criar("Barbearia", "wa-123", "prop-id");
        _repoMock.Setup(r => r.GetByIdAsync(est.Id, It.IsAny<CancellationToken>())).ReturnsAsync(est);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(new DesativarEstabelecimentoCommand(est.Id), CancellationToken.None);

        est.Ativo.Should().BeFalse();
        _repoMock.Verify(r => r.Update(est), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EstabelecimentoNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Estabelecimento?)null);

        var act = () => _handler.Handle(new DesativarEstabelecimentoCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Estabelecimento*");
    }
}

public class AtualizarNomeEstabelecimentoValidatorTests
{
    private readonly AtualizarNomeEstabelecimentoValidator _validator = new();

    [Fact]
    public void Validate_DadosValidos_Passa()
    {
        var result = _validator.Validate(new AtualizarNomeEstabelecimentoCommand(Guid.NewGuid(), "Nome"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_IdVazio_Falha()
    {
        var result = _validator.Validate(new AtualizarNomeEstabelecimentoCommand(Guid.Empty, "Nome"));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_NomeVazio_Falha(string nome)
    {
        var result = _validator.Validate(new AtualizarNomeEstabelecimentoCommand(Guid.NewGuid(), nome));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_NomeMuitoLongo_Falha()
    {
        var result = _validator.Validate(new AtualizarNomeEstabelecimentoCommand(Guid.NewGuid(), new string('a', 201)));
        result.IsValid.Should().BeFalse();
    }
}
