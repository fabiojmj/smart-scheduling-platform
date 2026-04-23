using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Entities;

public class RecurringSchedule : Entity
{
    public Guid EstablishmentId { get; private set; }
    public Guid ClientId        { get; private set; }
    public Guid EmployeeId      { get; private set; }
    public Guid ServiceId       { get; private set; }

    public RecurrenceFrequency Frequency    { get; private set; }
    public int                 Interval     { get; private set; }
    public DayOfWeek[]         DaysOfWeek   { get; private set; }
    public int?                DayOfMonth   { get; private set; }
    public TimeOnly            StartTime    { get; private set; }
    public DateOnly            StartsOn     { get; private set; }
    public DateOnly?           EndsOn       { get; private set; }
    public int?                MaxOccurrences { get; private set; }
    public bool                IsActive     { get; private set; }

    private readonly List<Appointment> _appointments = [];
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    private RecurringSchedule() { DaysOfWeek = []; }

    public static RecurringSchedule CreateWeekly(
        Guid establishmentId,
        Guid clientId,
        Guid employeeId,
        Guid serviceId,
        DayOfWeek[] daysOfWeek,
        TimeOnly startTime,
        DateOnly startsOn,
        DateOnly? endsOn       = null,
        int?      maxOccurrences = null,
        int       interval     = 1)
    {
        if (daysOfWeek.Length == 0)
            throw new DomainException("Informe ao menos um dia da semana para recorrencia semanal.");

        ValidateEndCondition(endsOn, maxOccurrences, startsOn);

        return new RecurringSchedule
        {
            EstablishmentId  = establishmentId,
            ClientId         = clientId,
            EmployeeId       = employeeId,
            ServiceId        = serviceId,
            Frequency        = RecurrenceFrequency.Weekly,
            Interval         = interval,
            DaysOfWeek       = daysOfWeek,
            StartTime        = startTime,
            StartsOn         = startsOn,
            EndsOn           = endsOn,
            MaxOccurrences   = maxOccurrences,
            IsActive         = true,
        };
    }

    public static RecurringSchedule CreateDaily(
        Guid      establishmentId,
        Guid      clientId,
        Guid      employeeId,
        Guid      serviceId,
        TimeOnly  startTime,
        DateOnly  startsOn,
        DateOnly? endsOn         = null,
        int?      maxOccurrences = null,
        int       interval       = 1)
    {
        ValidateEndCondition(endsOn, maxOccurrences, startsOn);

        return new RecurringSchedule
        {
            EstablishmentId = establishmentId,
            ClientId        = clientId,
            EmployeeId      = employeeId,
            ServiceId       = serviceId,
            Frequency       = RecurrenceFrequency.Daily,
            Interval        = interval,
            DaysOfWeek      = [],
            StartTime       = startTime,
            StartsOn        = startsOn,
            EndsOn          = endsOn,
            MaxOccurrences  = maxOccurrences,
            IsActive        = true,
        };
    }

    public static RecurringSchedule CreateMonthly(
        Guid      establishmentId,
        Guid      clientId,
        Guid      employeeId,
        Guid      serviceId,
        int       dayOfMonth,
        TimeOnly  startTime,
        DateOnly  startsOn,
        DateOnly? endsOn         = null,
        int?      maxOccurrences = null,
        int       interval       = 1)
    {
        if (dayOfMonth < 1 || dayOfMonth > 28)
            throw new DomainException("Dia do mes deve ser entre 1 e 28.");

        ValidateEndCondition(endsOn, maxOccurrences, startsOn);

        return new RecurringSchedule
        {
            EstablishmentId = establishmentId,
            ClientId        = clientId,
            EmployeeId      = employeeId,
            ServiceId       = serviceId,
            Frequency       = RecurrenceFrequency.Monthly,
            Interval        = interval,
            DaysOfWeek      = [],
            DayOfMonth      = dayOfMonth,
            StartTime       = startTime,
            StartsOn        = startsOn,
            EndsOn          = endsOn,
            MaxOccurrences  = maxOccurrences,
            IsActive        = true,
        };
    }

    public void Cancel()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public void UpdateEndCondition(DateOnly? endsOn, int? maxOccurrences)
    {
        ValidateEndCondition(endsOn, maxOccurrences, StartsOn);
        EndsOn         = endsOn;
        MaxOccurrences = maxOccurrences;
        MarkAsUpdated();
    }

    public IReadOnlyList<DateOnly> GenerateOccurrences(
        DateOnly from,
        DateOnly until,
        int      maxResults = 52)
    {
        var results   = new List<DateOnly>();
        var current   = StartsOn > from ? StartsOn : from;
        var hardLimit = EndsOn.HasValue && EndsOn < until ? EndsOn.Value : until;

        while (current <= hardLimit && results.Count < maxResults)
        {
            if (MaxOccurrences.HasValue && results.Count >= MaxOccurrences.Value)
                break;

            if (MatchesDate(current))
                results.Add(current);

            current = current.AddDays(1);
        }

        return results;
    }

    private bool MatchesDate(DateOnly date)
    {
        var daysSinceStart = date.DayNumber - StartsOn.DayNumber;

        return Frequency switch
        {
            RecurrenceFrequency.Daily =>
                daysSinceStart % Interval == 0,

            RecurrenceFrequency.Weekly =>
                daysSinceStart % (Interval * 7) < 7 &&
                DaysOfWeek.Contains(date.DayOfWeek),

            RecurrenceFrequency.Monthly =>
                DayOfMonth.HasValue &&
                date.Day == DayOfMonth.Value &&
                (date.Year - StartsOn.Year) * 12 + (date.Month - StartsOn.Month) % Interval == 0,

            _ => false
        };
    }

    private static void ValidateEndCondition(DateOnly? endsOn, int? maxOccurrences, DateOnly startsOn)
    {
        if (endsOn.HasValue && endsOn.Value <= startsOn)
            throw new DomainException("Data de termino deve ser apos a data de inicio.");

        if (maxOccurrences.HasValue && maxOccurrences.Value < 1)
            throw new DomainException("Numero de ocorrencias deve ser maior que zero.");
    }
}
