using FluentAssertions;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Tests.ValueObjects;

public class TimeSlotTests
{
    private static DateTime FutureDate => DateTime.UtcNow.AddDays(1).Date.AddHours(10);

    [Fact]
    public void Create_ComDadosValidos_CriaComSucesso()
    {
        var slot = TimeSlot.Create(FutureDate, 60);
        slot.Start.Should().Be(FutureDate);
        slot.End.Should().Be(FutureDate.AddMinutes(60));
        slot.DurationMinutes.Should().Be(60);
    }

    [Fact]
    public void Create_ComStartNoPassado_LancaArgumentException()
    {
        var pastDate = DateTime.UtcNow.AddSeconds(-1);
        var act = () => TimeSlot.Create(pastDate, 60);
        act.Should().Throw<ArgumentException>().WithMessage("*passado*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Create_ComDuracaoInvalida_LancaArgumentException(int duration)
    {
        var act = () => TimeSlot.Create(FutureDate, duration);
        act.Should().Throw<ArgumentException>().WithMessage("*maior que zero*");
    }

    [Fact]
    public void OverlapsWith_QuandoSobrepoem_RetornaVerdadeiro()
    {
        var slot1 = TimeSlot.Create(FutureDate, 60);
        var slot2 = TimeSlot.Create(FutureDate.AddMinutes(30), 60);
        slot1.OverlapsWith(slot2).Should().BeTrue();
    }

    [Fact]
    public void OverlapsWith_QuandoNaoSobrepoem_RetornaFalso()
    {
        var slot1 = TimeSlot.Create(FutureDate, 30);
        var slot2 = TimeSlot.Create(FutureDate.AddHours(2), 30);
        slot1.OverlapsWith(slot2).Should().BeFalse();
    }

    [Fact]
    public void OverlapsWith_QuandoAdjacentesSemSobreposicao_RetornaFalso()
    {
        var slot1 = TimeSlot.Create(FutureDate, 60);
        var slot2 = TimeSlot.Create(FutureDate.AddMinutes(60), 60);
        slot1.OverlapsWith(slot2).Should().BeFalse();
    }

    [Fact]
    public void Equals_ComMesmoSlot_RetornaVerdadeiro()
    {
        var s1 = TimeSlot.Create(FutureDate, 30);
        var s2 = TimeSlot.Create(FutureDate, 30);
        s1.Should().Be(s2);
        s1.GetHashCode().Should().Be(s2.GetHashCode());
    }

    [Fact]
    public void Equals_ComSlotDiferente_RetornaFalso()
    {
        var s1 = TimeSlot.Create(FutureDate, 30);
        var s2 = TimeSlot.Create(FutureDate.AddMinutes(30), 30);
        s1.Should().NotBe(s2);
    }

    [Fact]
    public void Equals_ComTiposDiferentes_RetornaFalso()
    {
        var slot = TimeSlot.Create(FutureDate, 30);
        slot.Equals("nao sou timeslot").Should().BeFalse();
    }
}
