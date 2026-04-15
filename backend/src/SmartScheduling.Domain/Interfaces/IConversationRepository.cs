using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface IConversationRepository : IRepository<Conversation>
{
    Task<Conversation?> GetActiveByPhoneAsync(string phoneNumber, Guid establishmentId, CancellationToken cancellationToken = default);
}
