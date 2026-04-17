using FluentAssertions;
using Moq;
using SmartScheduling.Application.Clientes.Queries;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Clientes;

public class ObterClienteHandlerTests
{
    private readonly Mock<IClienteRepository> _repoMock = new();
    private readonly ObterClienteHandler _handler;

    public ObterClienteHandlerTests()
    {
        _handler = new ObterClienteHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ClienteExistente_RetornaDto()
    {
        var estId = Guid.NewGuid();
        var cliente = Cliente.Criar("João", "11987654321", estId, "joao@test.com");
        _repoMock.Setup(r => r.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);

        var result = await _handler.Handle(new ObterClienteQuery(cliente.Id), CancellationToken.None);

        result.Id.Should().Be(cliente.Id);
        result.Nome.Should().Be("João");
        result.Telefone.Should().Be("11987654321");
        result.Email.Should().Be("joao@test.com");
        result.EstabelecimentoId.Should().Be(estId);
    }

    [Fact]
    public async Task Handle_ClienteNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        var act = () => _handler.Handle(new ObterClienteQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Cliente*");
    }
}

public class ListarClientesHandlerTests
{
    private readonly Mock<IClienteRepository> _repoMock = new();
    private readonly ListarClientesHandler _handler;

    public ListarClientesHandlerTests()
    {
        _handler = new ListarClientesHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_RetornaListaDeClienteDto()
    {
        var estId = Guid.NewGuid();
        var clientes = new List<Cliente>
        {
            Cliente.Criar("João", "11987654321", estId),
            Cliente.Criar("Maria", "11912345678", estId, "maria@test.com")
        };
        _repoMock.Setup(r => r.ListarPorEstabelecimentoAsync(estId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientes);

        var result = await _handler.Handle(new ListarClientesQuery(estId), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Nome.Should().Be("João");
        result[1].Email.Should().Be("maria@test.com");
    }

    [Fact]
    public async Task Handle_SemClientes_RetornaVazio()
    {
        var estId = Guid.NewGuid();
        _repoMock.Setup(r => r.ListarPorEstabelecimentoAsync(estId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Cliente>());

        var result = await _handler.Handle(new ListarClientesQuery(estId), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
