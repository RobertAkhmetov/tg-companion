namespace TgCompanion.Domain.Interfaces;

/// <summary>
/// Интерфейс для работы с Telegram Bot API
/// </summary>
public interface ITelegramBotService
{
    /// <summary>
    /// Отправляет текстовое сообщение
    /// </summary>
    Task SendTextMessageAsync(
        long chatId,
        string text,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправляет индикатор "печатает..."
    /// </summary>
    Task SendTypingActionAsync(
        long chatId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Запускает получение обновлений от Telegram
    /// </summary>
    Task StartReceivingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Останавливает получение обновлений
    /// </summary>
    Task StopReceivingAsync(CancellationToken cancellationToken = default);
}

