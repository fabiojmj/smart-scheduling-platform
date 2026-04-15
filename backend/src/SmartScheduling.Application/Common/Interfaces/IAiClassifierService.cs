namespace SmartScheduling.Application.Common.Interfaces;

public record ClassificationResult(string Intent, string? ServiceName, string? EmployeeName, DateTime? PreferredDate, TimeOnly? PreferredTime, string? BotReply);

public interface IAiClassifierService
{
    Task<ClassificationResult> ClassifyAsync(string message, IEnumerable<string> history, IEnumerable<string> services, CancellationToken cancellationToken = default);
}
