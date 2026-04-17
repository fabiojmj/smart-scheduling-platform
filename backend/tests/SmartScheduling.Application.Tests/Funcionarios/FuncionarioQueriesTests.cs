using FluentAssertions;
using Moq;
using SmartScheduling.Application.Funcionarios.Queries.ListarFuncionarios;
using SmartScheduling.Application.Funcionarios.Queries.ObterFuncionario;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Application.Tests.Funcionarios;

public class ListarFuncionariosHandlerTests
{
    private readonly Mock<IFuncionarioRepository> _repoMock = new();
    private readonly ListarFuncionariosHandler _handler;

    public ListarFuncionariosHandlerTests()
    {
        _handler = new ListarFuncionariosHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_RetornaListaDeFuncionarioDto()
    {
        var estId = Guid.NewGuid();
        var f = Funcionario.Criar("Ana", "ana@test.com", estId);
        f.AdicionarHorario(WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0)));

        _repoMock.Setup(r => r.ListarPorEstabelecimentoAsync(estId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Funcionario> { f });

        var result = await _handler.Handle(new ListarFuncionariosQuery(estId), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Nome.Should().Be("Ana");
        result[0].Email.Should().Be("ana@test.com");
        result[0].Ativo.Should().BeTrue();
        result[0].Horarios.Should().HaveCount(1);
        result[0].Horarios[0].DiaSemana.Should().Be("Monday");
    }

    [Fact]
    public async Task Handle_SemFuncionarios_RetornaVazio()
    {
        var estId = Guid.NewGuid();
        _repoMock.Setup(r => r.ListarPorEstabelecimentoAsync(estId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Funcionario>());

        var result = await _handler.Handle(new ListarFuncionariosQuery(estId), CancellationToken.None);

        result.Should().BeEmpty();
    }
}

public class ObterFuncionarioHandlerTests
{
    private readonly Mock<IFuncionarioRepository> _repoMock = new();
    private readonly ObterFuncionarioHandler _handler;

    public ObterFuncionarioHandlerTests()
    {
        _handler = new ObterFuncionarioHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_FuncionarioExistente_RetornaDto()
    {
        var f = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        _repoMock.Setup(r => r.ObterComDetalhesAsync(f.Id, It.IsAny<CancellationToken>())).ReturnsAsync(f);

        var result = await _handler.Handle(new ObterFuncionarioQuery(f.Id), CancellationToken.None);

        result.Id.Should().Be(f.Id);
        result.Nome.Should().Be("Ana");
        result.Email.Should().Be("ana@test.com");
        result.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_FuncionarioNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.ObterComDetalhesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Funcionario?)null);

        var act = () => _handler.Handle(new ObterFuncionarioQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Funcionario*");
    }
}
