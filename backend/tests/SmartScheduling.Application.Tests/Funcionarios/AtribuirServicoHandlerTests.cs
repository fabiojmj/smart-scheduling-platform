using FluentAssertions;
using Moq;
using SmartScheduling.Application.Funcionarios.Commands.AtribuirServico;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Funcionarios;

public class AtribuirServicoHandlerTests
{
    private readonly Mock<IFuncionarioRepository> _funcionarioRepoMock = new();
    private readonly Mock<IServicoRepository> _servicoRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly AtribuirServicoHandler _handler;

    public AtribuirServicoHandlerTests()
    {
        _handler = new AtribuirServicoHandler(_funcionarioRepoMock.Object, _servicoRepoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_DadosValidos_AtribuiServico()
    {
        var estId = Guid.NewGuid();
        var funcionario = Funcionario.Criar("Ana", "ana@test.com", estId);
        var servico = Servico.Criar("Corte", 30, 50m, estId);

        _funcionarioRepoMock.Setup(r => r.ObterComDetalhesAsync(funcionario.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(funcionario);
        _servicoRepoMock.Setup(r => r.GetByIdAsync(servico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servico);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(new AtribuirServicoCommand(funcionario.Id, servico.Id), CancellationToken.None);

        funcionario.Servicos.Should().Contain(s => s.Id == servico.Id);
        _funcionarioRepoMock.Verify(r => r.Update(funcionario), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FuncionarioNaoEncontrado_LancaDomainException()
    {
        _funcionarioRepoMock.Setup(r => r.ObterComDetalhesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Funcionario?)null);

        var act = () => _handler.Handle(
            new AtribuirServicoCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Funcionario*");
    }

    [Fact]
    public async Task Handle_ServicoNaoEncontrado_LancaDomainException()
    {
        var funcionario = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        _funcionarioRepoMock.Setup(r => r.ObterComDetalhesAsync(funcionario.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(funcionario);
        _servicoRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Servico?)null);

        var act = () => _handler.Handle(
            new AtribuirServicoCommand(funcionario.Id, Guid.NewGuid()),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Servico*");
    }
}
