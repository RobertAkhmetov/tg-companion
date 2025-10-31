namespace TgCompanion.Application.DTOs;

/// <summary>
/// DTO для входящего сообщения из Telegram
/// </summary>
public sealed record TelegramMessageDto(
    long ChatId,
    long MessageId,
    long UserId,
    string UserName,
    string? FirstName,
    string? LastName,
    string Text,
    DateTime ReceivedAt);

