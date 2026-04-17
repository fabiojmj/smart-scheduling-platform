using FluentAssertions;
using Moq;
using SmartScheduling.Application.Funcionarios.Commands.AdicionarHorario;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Funcionarios;

public class AdicionarHorarioHandlerTests
{
    private readonly Mock<IFuncionarioRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly AdicionarHorarioHandler _handler;

    public AdicionarHorarioHandlerTests()
    {
        _handler = new AdicionarHorarioHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_FuncionarioEncontrado_AdicionaHorario()
    {
        var funcionario = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        _repoMock.Setup(r => r.GetByIdAsync(funcionario.Id, It.IsAny<CancellationToken>())).ReturnsAsync(funcionario);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new AdicionarHorarioCommand(
            funcionario.Id, DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0));
        await _handler.Handle(command, CancellationToken.None);

        funcionario.Horarios.Should().HaveCount(1);
        _repoMock.Verify(r => r.Update(funcionario), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FuncionarioNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Funcionario?)null);

        var act = () => _handler.Handle(
            new AdicionarHorarioCommand(Guid.NewGuid(), DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0)),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Funcionario*");
    }
}

public class AdicionarHorarioValidatorTests
{
    private readonly AdicionarHorarioValidator _validator = new();

    [Fact]
    public void Validate_DadosValidos_Passa()
    {
        var result = _validator.Validate(new AdicionarHorarioCommand(
            Guid.NewGuid(), DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0)));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_FuncionarioIdVazio_Falha()
    {
        var result = _validator.Validate(new AdicionarHorarioCommand(
            Guid.Empty, DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0)));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_HoraFimAntesDaInicio_Falha()
    {
        var result = _validator.Validate(new AdicionarHorarioCommand(
            Guid.NewGuid(), DayOfWeek.Monday, new TimeOnly(18, 0), new TimeOnly(8, 0)));
        result.IsValid.Should().BeFalse();
    }
}
