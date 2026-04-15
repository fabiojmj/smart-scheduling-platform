namespace SmartScheduling.Domain.ValueObjects;

public sealed class TimeSlot
{
    public DateTime Start { get; }
    public DateTime End { get; }
    public int DurationMinutes => (int)(End - Start).TotalMinutes;

    private TimeSlot(DateTime start, DateTime end) { Start = start; End = end; }

    public static TimeSlot Create(DateTime start, int durationMinutes)
    {
        if (start < DateTime.UtcNow)
            throw new ArgumentException("O horario de inicio nao pode ser no passado.", nameof(start));
        if (durationMinutes <= 0)
            throw new ArgumentException("Duracao deve ser maior que zero.", nameof(durationMinutes));
        return new TimeSlot(start, start.AddMinutes(durationMinutes));
    }

    public bool OverlapsWith(TimeSlot other) => Start < other.End && End > other.Start;
    public override bool Equals(object? obj) => obj is TimeSlot other && Start == other.Start && End == other.End;
    public override int GetHashCode() => HashCode.Combine(Start, End);
}
