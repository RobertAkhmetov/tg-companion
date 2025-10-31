namespace TgCompanion.Domain.Entities;

/// <summary>
/// Представляет контекст разговора в чате
/// </summary>
public sealed class Conversation
{
    private readonly List<ChatMessage> _messages = [];

    public long ChatId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastActivityAt { get; private set; }
    public IReadOnlyList<ChatMessage> Messages => _messages.AsReadOnly();

    public static Conversation Create(long chatId)
    {
        var now = DateTime.UtcNow;
        return new Conversation
        {
            ChatId = chatId,
            CreatedAt = now,
            LastActivityAt = now
        };
    }

    public void AddMessage(ChatMessage message)
    {
        if (message.ChatId != ChatId)
        {
            throw new InvalidOperationException("Message chat ID does not match conversation chat ID");
        }

        _messages.Add(message);
        LastActivityAt = DateTime.UtcNow;
    }

    public void ClearHistory()
    {
        _messages.Clear();
        LastActivityAt = DateTime.UtcNow;
    }

    public IEnumerable<ChatMessage> GetRecentMessages(int count)
    {
        return _messages.TakeLast(count);
    }
}

