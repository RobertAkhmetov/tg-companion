namespace TgCompanion.Domain.Entities;

/// <summary>
/// Представляет сообщение в чате
/// </summary>
public sealed class ChatMessage
{
    public long Id { get; init; }
    public long ChatId { get; init; }
    public long UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public MessageRole Role { get; init; }

    public static ChatMessage CreateUserMessage(long chatId, long userId, string userName, string text)
    {
        return new ChatMessage
        {
            ChatId = chatId,
            UserId = userId,
            UserName = userName,
            Text = text,
            Timestamp = DateTime.UtcNow,
            Role = MessageRole.User
        };
    }

    public static ChatMessage CreateAssistantMessage(long chatId, string text)
    {
        return new ChatMessage
        {
            ChatId = chatId,
            Text = text,
            Timestamp = DateTime.UtcNow,
            Role = MessageRole.Assistant
        };
    }
}

