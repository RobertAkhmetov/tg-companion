namespace TgCompanion.Infrastructure.Configuration;

/// <summary>
/// Настройки Telegram бота
/// </summary>
public sealed class TelegramSettings
{
    public const string SectionName = "Telegram";

    /// <summary>
    /// Токен Telegram бота
    /// </summary>
    public string BotToken { get; set; } = string.Empty;

    /// <summary>
    /// Имя бота
    /// </summary>
    public string BotUsername { get; set; } = string.Empty;
}

