using FluentAssertions;
using Moq;
using SmartScheduling.Application.Servicos.Queries;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Servicos;

public class ObterServicoHandlerTests
{
    private readonly Mock<IServicoRepository> _repoMock = new();
    private readonly ObterServicoHandler _handler;

    public ObterServicoHandlerTests()
    {
        _handler = new ObterServicoHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ServicoExistente_RetornaDto()
    {
        var estId = Guid.NewGuid();
        var servico = Servico.Criar("Corte", 30, 50m, estId, "desc");
        _repoMock.Setup(r => r.GetByIdAsync(servico.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servico);

        var result = await _handler.Handle(new ObterServicoQuery(servico.Id), CancellationToken.None);

        result.Id.Should().Be(servico.Id);
        result.Nome.Should().Be("Corte");
        result.Descricao.Should().Be("desc");
        result.DuracaoMinutos.Should().Be(30);
        result.Preco.Should().Be(50m);
        result.EstabelecimentoId.Should().Be(estId);
        result.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ServicoNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Servico?)null);

        var act = () => _handler.Handle(new ObterServicoQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Servico*");
    }
}

public class ListarServicosHandlerTests
{
    private readonly Mock<IServicoRepository> _repoMock = new();
    private readonly ListarServicosHandler _handler;

    public ListarServicosHandlerTests()
    {
        _handler = new ListarServicosHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_RetornaListaDeServicoDto()
    {
        var estId = Guid.NewGuid();
        var servicos = new List<Servico>
        {
            Servico.Criar("Corte", 30, 50m, estId),
            Servico.Criar("Barba", 20, 30m, estId, "barba completa")
        };
        _repoMock.Setup(r => r.ListarPorEstabelecimentoAsync(estId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicos);

        var result = await _handler.Handle(new ListarServicosQuery(estId), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Nome.Should().Be("Corte");
        result[1].Descricao.Should().Be("barba completa");
    }

    [Fact]
    public async Task Handle_SemServicos_RetornaVazio()
    {
        var estId = Guid.NewGuid();
        _repoMock.Setup(r => r.ListarPorEstabelecimentoAsync(estId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Servico>());

        var result = await _handler.Handle(new ListarServicosQuery(estId), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
