using FluentAssertions;
using Moq;
using SmartScheduling.Application.Servicos.Commands.AtualizarServico;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Servicos;

public class AtualizarPrecoHandlerTests
{
    private readonly Mock<IServicoRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly AtualizarPrecoHandler _handler;

    public AtualizarPrecoHandlerTests()
    {
        _handler = new AtualizarPrecoHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_ServicoEncontrado_AtualizaPreco()
    {
        var servico = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        _repoMock.Setup(r => r.GetByIdAsync(servico.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servico);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(new AtualizarPrecoCommand(servico.Id, 75m), CancellationToken.None);

        servico.Preco.Should().Be(75m);
        _repoMock.Verify(r => r.Update(servico), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServicoNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Servico?)null);

        var act = () => _handler.Handle(new AtualizarPrecoCommand(Guid.NewGuid(), 75m), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Servico*");
    }
}

public class AtualizarDuracaoHandlerTests
{
    private readonly Mock<IServicoRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly AtualizarDuracaoHandler _handler;

    public AtualizarDuracaoHandlerTests()
    {
        _handler = new AtualizarDuracaoHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_ServicoEncontrado_AtualizaDuracao()
    {
        var servico = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        _repoMock.Setup(r => r.GetByIdAsync(servico.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servico);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(new AtualizarDuracaoCommand(servico.Id, 60), CancellationToken.None);

        servico.DuracaoMinutos.Should().Be(60);
        _repoMock.Verify(r => r.Update(servico), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServicoNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Servico?)null);

        var act = () => _handler.Handle(new AtualizarDuracaoCommand(Guid.NewGuid(), 60), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Servico*");
    }
}

public class DesativarServicoHandlerTests
{
    private readonly Mock<IServicoRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly DesativarServicoHandler _handler;

    public DesativarServicoHandlerTests()
    {
        _handler = new DesativarServicoHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_ServicoEncontrado_Desativa()
    {
        var servico = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        _repoMock.Setup(r => r.GetByIdAsync(servico.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servico);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(new DesativarServicoCommand(servico.Id), CancellationToken.None);

        servico.Ativo.Should().BeFalse();
        _repoMock.Verify(r => r.Update(servico), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServicoNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Servico?)null);

        var act = () => _handler.Handle(new DesativarServicoCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Servico*");
    }
}
