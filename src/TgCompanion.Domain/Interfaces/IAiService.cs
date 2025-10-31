using TgCompanion.Domain.Entities;

namespace TgCompanion.Domain.Interfaces;

/// <summary>
/// Интерфейс для работы с AI-сервисом (Ollama)
/// </summary>
public interface IAiService
{
    /// <summary>
    /// Генерирует ответ на основе истории сообщений
    /// </summary>
    Task<string> GenerateResponseAsync(
        IEnumerable<ChatMessage> conversationHistory,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Генерирует ответ в потоковом режиме
    /// </summary>
    IAsyncEnumerable<string> GenerateResponseStreamAsync(
        IEnumerable<ChatMessage> conversationHistory,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет доступность AI-сервиса
    /// </summary>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

