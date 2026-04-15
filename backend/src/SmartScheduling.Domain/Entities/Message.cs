using SmartScheduling.Domain.Enums;

namespace SmartScheduling.Domain.Entities;

public class Message : Entity
{
    public Guid ConversationId { get; private set; }
    public string Content { get; private set; }
    public MessageType Type { get; private set; }
    public bool IsFromClient { get; private set; }
    public string? TranscriptionText { get; private set; }

    private Message() { Content = default!; }

    public static Message Create(Guid conversationId, string content, MessageType type, bool isFromClient, string? transcription = null) =>
        new() { ConversationId = conversationId, Content = content, Type = type, IsFromClient = isFromClient, TranscriptionText = transcription };
}
