using FluentAssertions;
using Moq;
using SmartScheduling.Application.Clientes.Commands.CriarCliente;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Clientes;

public class CriarClienteHandlerTests
{
    private readonly Mock<IClienteRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly CriarClienteHandler _handler;

    public CriarClienteHandlerTests()
    {
        _handler = new CriarClienteHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_DadosValidos_CriaClienteERetornaId()
    {
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(
            new CriarClienteCommand("João", "11987654321", Guid.NewGuid(), "joao@test.com"),
            CancellationToken.None);

        result.Should().NotBeEmpty();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SemEmail_CriaClienteSemEmail()
    {
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(
            new CriarClienteCommand("João", "11987654321", Guid.NewGuid()),
            CancellationToken.None);

        result.Should().NotBeEmpty();
    }
}
