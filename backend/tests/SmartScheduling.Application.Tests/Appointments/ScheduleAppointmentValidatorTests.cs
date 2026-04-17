using FluentAssertions;
using SmartScheduling.Application.Appointments.Commands.ScheduleAppointment;

namespace SmartScheduling.Application.Tests.Appointments;

public class ScheduleAppointmentValidatorTests
{
    private readonly ScheduleAppointmentValidator _validator = new();

    [Fact]
    public void Validate_DadosValidos_Passa()
    {
        var cmd = new ScheduleAppointmentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1), null);
        _validator.Validate(cmd).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ClientIdVazio_Falha()
    {
        var cmd = new ScheduleAppointmentCommand(
            Guid.Empty, Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1), null);
        _validator.Validate(cmd).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmployeeIdVazio_Falha()
    {
        var cmd = new ScheduleAppointmentCommand(
            Guid.NewGuid(), Guid.Empty, Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1), null);
        _validator.Validate(cmd).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ServiceIdVazio_Falha()
    {
        var cmd = new ScheduleAppointmentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.Empty,
            DateTime.UtcNow.AddDays(1), null);
        _validator.Validate(cmd).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_DataNoPassado_Falha()
    {
        var cmd = new ScheduleAppointmentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1), null);
        _validator.Validate(cmd).IsValid.Should().BeFalse();
    }
}
