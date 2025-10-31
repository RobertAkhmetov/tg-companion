using TgCompanion.Domain.Entities;

namespace TgCompanion.Domain.Interfaces;

/// <summary>
/// Репозиторий для управления разговорами
/// </summary>
public interface IConversationRepository
{
    /// <summary>
    /// Получает разговор по ID чата
    /// </summary>
    Task<Conversation?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет разговор
    /// </summary>
    Task SaveAsync(Conversation conversation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет разговор по ID чата
    /// </summary>
    Task DeleteAsync(long chatId, CancellationToken cancellationToken = default);
}

