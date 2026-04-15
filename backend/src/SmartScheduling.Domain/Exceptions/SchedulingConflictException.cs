namespace SmartScheduling.Domain.Exceptions;

public class SchedulingConflictException : DomainException
{
    public SchedulingConflictException(string employeeName, DateTime start, DateTime end)
        : base($"O funcionario '{employeeName}' ja possui agendamento entre {start:HH:mm} e {end:HH:mm}.") { }
}
