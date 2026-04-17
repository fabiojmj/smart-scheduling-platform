using FluentAssertions;
using Moq;
using SmartScheduling.Application.Appointments.Commands.CancelAppointment;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Application.Tests.Appointments;

public class CancelAppointmentHandlerTests
{
    private readonly Mock<IAgendamentoRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly CancelAppointmentHandler _handler;

    public CancelAppointmentHandlerTests()
    {
        _handler = new CancelAppointmentHandler(_repoMock.Object, _uowMock.Object);
    }

    private static Agendamento CriarAgendamentoValido()
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
    public async Task Handle_AgendamentoValido_Cancela()
    {
        var agendamento = CriarAgendamentoValido();
        _repoMock.Setup(r => r.GetByIdAsync(agendamento.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agendamento);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(new CancelAppointmentCommand(agendamento.Id, "Motivo teste"), CancellationToken.None);

        agendamento.Status.Should().Be(SmartScheduling.Domain.Enums.StatusAgendamento.Cancelado);
        _repoMock.Verify(r => r.Update(agendamento), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AgendamentoNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agendamento?)null);

        var act = () => _handler.Handle(
            new CancelAppointmentCommand(Guid.NewGuid(), "Motivo"),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Agendamento*");
    }
}
