using FluentAssertions;
using Moq;
using SmartScheduling.Application.Appointments.Queries.GetAppointmentsByEmployee;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Application.Tests.Appointments;

public class GetAppointmentsByEmployeeHandlerTests
{
    private readonly Mock<IAgendamentoRepository> _repoMock = new();
    private readonly GetAppointmentsByEmployeeHandler _handler;

    public GetAppointmentsByEmployeeHandlerTests()
    {
        _handler = new GetAppointmentsByEmployeeHandler(_repoMock.Object);
    }

    private static Agendamento CriarAgendamento()
    {
        var estId = Guid.NewGuid();
        var servico = Servico.Criar("Corte", 30, 50m, estId);
        var funcionario = Funcionario.Criar("Ana", "ana@test.com", estId);
        funcionario.AtribuirServico(servico);
        funcionario.AdicionarHorario(WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0)));
        var cliente = Cliente.Criar("João", "11987654321", estId);

        var now = DateTime.UtcNow;
        int days = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
        if (days == 0) days = 7;
        var start = now.Date.AddDays(days).AddHours(10);

        return Agendamento.Agendar(cliente, funcionario, servico, TimeSlot.Create(start, 30));
    }

    [Fact]
    public async Task Handle_RetornaListaDeAppointmentDto()
    {
        var employeeId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var agendamento = CriarAgendamento();

        _repoMock.Setup(r => r.ObterPorFuncionarioAsync(employeeId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Agendamento> { agendamento });

        var result = await _handler.Handle(
            new GetAppointmentsByEmployeeQuery(employeeId, date),
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(agendamento.Id);
        result[0].ClientName.Should().Be("-");
        result[0].ServiceName.Should().Be("-");
        result[0].Status.Should().Be("Pendente");
    }

    [Fact]
    public async Task Handle_ListaVazia_RetornaVazio()
    {
        var employeeId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        _repoMock.Setup(r => r.ObterPorFuncionarioAsync(employeeId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Agendamento>());

        var result = await _handler.Handle(
            new GetAppointmentsByEmployeeQuery(employeeId, date),
            CancellationToken.None);

        result.Should().BeEmpty();
    }
}
