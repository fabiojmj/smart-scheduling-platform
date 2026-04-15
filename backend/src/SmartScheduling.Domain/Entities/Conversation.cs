using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Entities;

public class Conversation : Entity
{
    public PhoneNumber ClientPhone { get; private set; }
    public Guid EstablishmentId { get; private set; }
    public ConversationStatus Status { get; private set; }
    public Guid? PendingAppointmentId { get; private set; }

    private readonly List<Message> _messages = [];
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    private Conversation() { ClientPhone = default!; }

    public static Conversation Start(string clientPhone, Guid establishmentId) =>
        new() { ClientPhone = PhoneNumber.Create(clientPhone), EstablishmentId = establishmentId, Status = ConversationStatus.Active };

    public void AddMessage(string content, MessageType type, bool isFromClient) =>
        _messages.Add(Message.Create(Id, content, type, isFromClient));

    public void WaitForConfirmation(Guid id) { Status = ConversationStatus.WaitingForConfirmation; PendingAppointmentId = id; MarkAsUpdated(); }
    public void Complete() { Status = ConversationStatus.Completed; MarkAsUpdated(); }
    public void Abandon() { Status = ConversationStatus.Abandoned; MarkAsUpdated(); }
}
