using FluentAssertions;
using Moq;
using SmartScheduling.Application.Clientes.Commands.AtualizarCliente;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Clientes;

public class AtualizarClienteHandlerTests
{
    private readonly Mock<IClienteRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly AtualizarNomeClienteHandler _handler;

    public AtualizarClienteHandlerTests()
    {
        _handler = new AtualizarNomeClienteHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_ClienteEncontrado_AtualizaNome()
    {
        var cliente = Cliente.Criar("João", "11987654321", Guid.NewGuid());
        _repoMock.Setup(r => r.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(new AtualizarNomeClienteCommand(cliente.Id, "Novo Nome"), CancellationToken.None);

        cliente.Nome.Should().Be("Novo Nome");
        _repoMock.Verify(r => r.Update(cliente), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ClienteNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        var act = () => _handler.Handle(
            new AtualizarNomeClienteCommand(Guid.NewGuid(), "Nome"),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Cliente*");
    }
}

public class AtualizarNomeClienteValidatorTests
{
    private readonly AtualizarNomeClienteValidator _validator = new();

    [Fact]
    public void Validate_DadosValidos_Passa()
    {
        var result = _validator.Validate(new AtualizarNomeClienteCommand(Guid.NewGuid(), "João"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_IdVazio_Falha()
    {
        var result = _validator.Validate(new AtualizarNomeClienteCommand(Guid.Empty, "João"));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_NomeVazio_Falha(string nome)
    {
        var result = _validator.Validate(new AtualizarNomeClienteCommand(Guid.NewGuid(), nome));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_NomeMuitoLongo_Falha()
    {
        var result = _validator.Validate(new AtualizarNomeClienteCommand(Guid.NewGuid(), new string('a', 201)));
        result.IsValid.Should().BeFalse();
    }
}
