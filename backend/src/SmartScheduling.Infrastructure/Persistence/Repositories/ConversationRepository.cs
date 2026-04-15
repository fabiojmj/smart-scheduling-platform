using Microsoft.EntityFrameworkCore;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Persistence.Repositories;

public class ConversationRepository(AppDbContext context) : Repository<Conversation>(context), IConversationRepository
{
    public async Task<Conversation?> GetActiveByPhoneAsync(string phoneNumber, Guid establishmentId, CancellationToken cancellationToken = default) =>
        await Context.Conversations.Include(c => c.Messages)
            .FirstOrDefaultAsync(c =>
                c.ClientPhone.Value == phoneNumber && c.EstablishmentId == establishmentId
                && (c.Status == ConversationStatus.Active || c.Status == ConversationStatus.WaitingForConfirmation),
                cancellationToken);
}
