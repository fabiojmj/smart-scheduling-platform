using FluentAssertions;
using Moq;
using SmartScheduling.Application.Appointments.Commands.ScheduleAppointment;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Application.Tests.Appointments;

public class ScheduleAppointmentHandlerTests
{
    private readonly Mock<IRepository<Cliente>> _clienteRepoMock = new();
    private readonly Mock<IRepository<Funcionario>> _funcionarioRepoMock = new();
    private readonly Mock<IRepository<Servico>> _servicoRepoMock = new();
    private readonly Mock<IAgendamentoRepository> _agendamentoRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly ScheduleAppointmentHandler _handler;

    public ScheduleAppointmentHandlerTests()
    {
        _handler = new ScheduleAppointmentHandler(
            _clienteRepoMock.Object,
            _funcionarioRepoMock.Object,
            _servicoRepoMock.Object,
            _agendamentoRepoMock.Object,
            _uowMock.Object);
    }

    private static DateTime NextMonday(int hour = 10)
    {
        var now = DateTime.UtcNow;
        int days = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
        if (days == 0) days = 7;
        return now.Date.AddDays(days).AddHours(hour);
    }

    private static (Cliente cliente, Funcionario funcionario, Servico servico) CriarEntidades()
    {
        var estId = Guid.NewGuid();
        var servico = Servico.Criar("Corte", 30, 50m, estId);
        var funcionario = Funcionario.Criar("Ana", "ana@test.com", estId);
        funcionario.AtribuirServico(servico);
        funcionario.AdicionarHorario(WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0)));
        var cliente = Cliente.Criar("João", "11987654321", estId);
        return (cliente, funcionario, servico);
    }

    [Fact]
    public async Task Handle_DadosValidos_AgendaERetornaId()
    {
        var (cliente, funcionario, servico) = CriarEntidades();
        var start = NextMonday();

        _clienteRepoMock.Setup(r => r.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        _funcionarioRepoMock.Setup(r => r.GetByIdAsync(funcionario.Id, It.IsAny<CancellationToken>())).ReturnsAsync(funcionario);
        _servicoRepoMock.Setup(r => r.GetByIdAsync(servico.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servico);
        _agendamentoRepoMock.Setup(r => r.AddAsync(It.IsAny<Agendamento>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new ScheduleAppointmentCommand(cliente.Id, funcionario.Id, servico.Id, start, "obs");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        _agendamentoRepoMock.Verify(r => r.AddAsync(It.IsAny<Agendamento>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ClienteNaoEncontrado_LancaDomainException()
    {
        _clienteRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        var act = () => _handler.Handle(
            new ScheduleAppointmentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), NextMonday(), null),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Cliente*");
    }

    [Fact]
    public async Task Handle_FuncionarioNaoEncontrado_LancaDomainException()
    {
        var (cliente, _, _) = CriarEntidades();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        _funcionarioRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Funcionario?)null);

        var act = () => _handler.Handle(
            new ScheduleAppointmentCommand(cliente.Id, Guid.NewGuid(), Guid.NewGuid(), NextMonday(), null),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Funcionario*");
    }

    [Fact]
    public async Task Handle_ServicoNaoEncontrado_LancaDomainException()
    {
        var (cliente, funcionario, _) = CriarEntidades();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        _funcionarioRepoMock.Setup(r => r.GetByIdAsync(funcionario.Id, It.IsAny<CancellationToken>())).ReturnsAsync(funcionario);
        _servicoRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Servico?)null);

        var act = () => _handler.Handle(
            new ScheduleAppointmentCommand(cliente.Id, funcionario.Id, Guid.NewGuid(), NextMonday(), null),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Servico*");
    }
}
