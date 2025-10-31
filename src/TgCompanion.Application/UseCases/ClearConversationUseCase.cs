using Microsoft.Extensions.Logging;
using TgCompanion.Domain.Interfaces;

namespace TgCompanion.Application.UseCases;

/// <summary>
/// Use case для очистки истории разговора
/// </summary>
public sealed class ClearConversationUseCase
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ITelegramBotService _telegramBotService;
    private readonly ILogger<ClearConversationUseCase> _logger;

    public ClearConversationUseCase(
        IConversationRepository conversationRepository,
        ITelegramBotService telegramBotService,
        ILogger<ClearConversationUseCase> logger)
    {
        _conversationRepository = conversationRepository;
        _telegramBotService = telegramBotService;
        _logger = logger;
    }

    public async Task ExecuteAsync(long chatId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Clearing conversation for chat {ChatId}", chatId);

            await _conversationRepository.DeleteAsync(chatId, cancellationToken);

            await _telegramBotService.SendTextMessageAsync(
                chatId,
                "История разговора очищена. Начнем с чистого листа! 🔄",
                cancellationToken);

            _logger.LogInformation("Successfully cleared conversation for chat {ChatId}", chatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing conversation for chat {ChatId}", chatId);
            
            try
            {
                await _telegramBotService.SendTextMessageAsync(
                    chatId,
                    "Произошла ошибка при очистке истории разговора.",
                    cancellationToken);
            }
            catch
            {
                // Игнорируем ошибку отправки сообщения об ошибке
            }
        }
    }
}

