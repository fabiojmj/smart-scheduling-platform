namespace SmartScheduling.Application.Common.Interfaces;

public interface IWhatsAppService
{
    Task SendTextMessageAsync(string to, string message, CancellationToken cancellationToken = default);
    Task<string> TranscribeAudioAsync(string audioUrl, CancellationToken cancellationToken = default);
}
