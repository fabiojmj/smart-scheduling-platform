using FluentAssertions;
using Moq;
using SmartScheduling.Application.Auth.Commands.RegistrarUsuario;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Auth;

public class RegistrarUsuarioHandlerTests
{
    private readonly Mock<IUsuarioRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly RegistrarUsuarioHandler _handler;

    public RegistrarUsuarioHandlerTests()
    {
        _handler = new RegistrarUsuarioHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_DadosValidos_CriaUsuarioERetornaId()
    {
        _repoMock.Setup(r => r.EmailExisteAsync("novo@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(
            new RegistrarUsuarioCommand("João", "novo@test.com", "senha12345"),
            CancellationToken.None);

        result.Should().NotBeEmpty();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailJaCadastrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.EmailExisteAsync("existente@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _handler.Handle(
            new RegistrarUsuarioCommand("João", "existente@test.com", "senha12345"),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Email já cadastrado.");
    }
}
