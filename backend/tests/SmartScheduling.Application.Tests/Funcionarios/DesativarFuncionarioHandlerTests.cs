using FluentAssertions;
using Moq;
using SmartScheduling.Application.Funcionarios.Commands.DesativarFuncionario;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Funcionarios;

public class DesativarFuncionarioHandlerTests
{
    private readonly Mock<IFuncionarioRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly DesativarFuncionarioHandler _handler;

    public DesativarFuncionarioHandlerTests()
    {
        _handler = new DesativarFuncionarioHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_FuncionarioEncontrado_Desativa()
    {
        var funcionario = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        _repoMock.Setup(r => r.GetByIdAsync(funcionario.Id, It.IsAny<CancellationToken>())).ReturnsAsync(funcionario);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(new DesativarFuncionarioCommand(funcionario.Id), CancellationToken.None);

        funcionario.Ativo.Should().BeFalse();
        _repoMock.Verify(r => r.Update(funcionario), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FuncionarioNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Funcionario?)null);

        var act = () => _handler.Handle(new DesativarFuncionarioCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Funcionario*");
    }
}
