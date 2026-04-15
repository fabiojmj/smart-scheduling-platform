namespace SmartScheduling.Domain.ValueObjects;

public sealed class WorkingHours
{
    public DayOfWeek DayOfWeek { get; }
    public TimeOnly Start { get; }
    public TimeOnly End { get; }

    private WorkingHours(DayOfWeek dayOfWeek, TimeOnly start, TimeOnly end)
    { DayOfWeek = dayOfWeek; Start = start; End = end; }

    public static WorkingHours Create(DayOfWeek dayOfWeek, TimeOnly start, TimeOnly end)
    {
        if (end <= start)
            throw new ArgumentException("Horario de encerramento deve ser apos o inicio.");
        return new WorkingHours(dayOfWeek, start, end);
    }

    public bool IsWithin(TimeOnly time) => time >= Start && time <= End;
    public bool IsAvailableOn(DateTime dt) => dt.DayOfWeek == DayOfWeek && IsWithin(TimeOnly.FromDateTime(dt));
    public override string ToString() => $"{DayOfWeek}: {Start:HH:mm} - {End:HH:mm}";
}
