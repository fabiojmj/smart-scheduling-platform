using FluentAssertions;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Tests.ValueObjects;

public class WorkingHoursTests
{
    [Fact]
    public void Create_ComDadosValidos_CriaComSucesso()
    {
        var start = new TimeOnly(8, 0);
        var end = new TimeOnly(18, 0);
        var wh = WorkingHours.Create(DayOfWeek.Monday, start, end);
        wh.DayOfWeek.Should().Be(DayOfWeek.Monday);
        wh.Start.Should().Be(start);
        wh.End.Should().Be(end);
    }

    [Fact]
    public void Create_ComFimAntesDoComeço_LancaArgumentException()
    {
        var act = () => WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(18, 0), new TimeOnly(8, 0));
        act.Should().Throw<ArgumentException>().WithMessage("*apos*");
    }

    [Fact]
    public void Create_ComFimIgualAoInicio_LancaArgumentException()
    {
        var time = new TimeOnly(10, 0);
        var act = () => WorkingHours.Create(DayOfWeek.Monday, time, time);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IsWithin_QuandoDentroDoHorario_RetornaVerdadeiro()
    {
        var wh = WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0));
        wh.IsWithin(new TimeOnly(12, 0)).Should().BeTrue();
    }

    [Fact]
    public void IsWithin_QuandoNoLimiteInicio_RetornaVerdadeiro()
    {
        var wh = WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0));
        wh.IsWithin(new TimeOnly(8, 0)).Should().BeTrue();
    }

    [Fact]
    public void IsWithin_QuandoForaDoHorario_RetornaFalso()
    {
        var wh = WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0));
        wh.IsWithin(new TimeOnly(7, 59)).Should().BeFalse();
    }

    [Fact]
    public void IsAvailableOn_QuandoDiaEHorarioCorretos_RetornaVerdadeiro()
    {
        var wh = WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0));
        var monday = NextWeekday(DayOfWeek.Monday).Date.AddHours(10);
        wh.IsAvailableOn(monday).Should().BeTrue();
    }

    [Fact]
    public void IsAvailableOn_QuandoDiaDiferente_RetornaFalso()
    {
        var wh = WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0));
        var tuesday = NextWeekday(DayOfWeek.Tuesday).Date.AddHours(10);
        wh.IsAvailableOn(tuesday).Should().BeFalse();
    }

    [Fact]
    public void IsAvailableOn_QuandoHorarioForaDoRange_RetornaFalso()
    {
        var wh = WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0));
        var monday = NextWeekday(DayOfWeek.Monday).Date.AddHours(7);
        wh.IsAvailableOn(monday).Should().BeFalse();
    }

    [Fact]
    public void ToString_RetornaFormatoCorreto()
    {
        var wh = WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0));
        wh.ToString().Should().Contain("Monday").And.Contain("08:00").And.Contain("18:00");
    }

    private static DateTime NextWeekday(DayOfWeek day)
    {
        var now = DateTime.UtcNow;
        int daysUntil = ((int)day - (int)now.DayOfWeek + 7) % 7;
        if (daysUntil == 0) daysUntil = 7;
        return now.AddDays(daysUntil);
    }
}
