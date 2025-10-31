using Microsoft.Extensions.Logging;
using TgCompanion.Application.DTOs;
using TgCompanion.Domain.Entities;
using TgCompanion.Domain.Interfaces;

namespace TgCompanion.Application.UseCases;

/// <summary>
/// Use case для обработки входящих сообщений
/// </summary>
public sealed class ProcessMessageUseCase
{
    private readonly IAiService _aiService;
    private readonly IConversationRepository _conversationRepository;
    private readonly ITelegramBotService _telegramBotService;
    private readonly ILogger<ProcessMessageUseCase> _logger;
    private const int MaxContextMessages = 10;

    public ProcessMessageUseCase(
        IAiService aiService,
        IConversationRepository conversationRepository,
        ITelegramBotService telegramBotService,
        ILogger<ProcessMessageUseCase> logger)
    {
        _aiService = aiService;
        _conversationRepository = conversationRepository;
        _telegramBotService = telegramBotService;
        _logger = logger;
    }

    public async Task ExecuteAsync(TelegramMessageDto messageDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Processing message from user {UserId} in chat {ChatId}: {MessageText}",
                messageDto.UserId, messageDto.ChatId, messageDto.Text);

            // Получаем или создаем разговор
            var conversation = await _conversationRepository.GetByChatIdAsync(messageDto.ChatId, cancellationToken)
                               ?? Conversation.Create(messageDto.ChatId);

            // Добавляем сообщение пользователя
            var userMessage = ChatMessage.CreateUserMessage(
                messageDto.ChatId,
                messageDto.UserId,
                messageDto.UserName,
                messageDto.Text);

            conversation.AddMessage(userMessage);

            // Показываем индикатор "печатает..."
            await _telegramBotService.SendTypingActionAsync(messageDto.ChatId, cancellationToken);

            // Получаем контекст разговора (последние N сообщений)
            var contextMessages = conversation.GetRecentMessages(MaxContextMessages);

            // Генерируем ответ от AI
            var aiResponse = await _aiService.GenerateResponseAsync(contextMessages, cancellationToken);

            if (string.IsNullOrWhiteSpace(aiResponse))
            {
                _logger.LogWarning("AI service returned empty response");
                aiResponse = "Извините, не смог сгенерировать ответ. Попробуйте еще раз.";
            }

            // Добавляем ответ бота в разговор
            var assistantMessage = ChatMessage.CreateAssistantMessage(messageDto.ChatId, aiResponse);
            conversation.AddMessage(assistantMessage);

            // Сохраняем разговор
            await _conversationRepository.SaveAsync(conversation, cancellationToken);

            // Отправляем ответ пользователю
            await _telegramBotService.SendTextMessageAsync(messageDto.ChatId, aiResponse, cancellationToken);

            _logger.LogInformation("Successfully processed message in chat {ChatId}", messageDto.ChatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message in chat {ChatId}", messageDto.ChatId);
            
            try
            {
                await _telegramBotService.SendTextMessageAsync(
                    messageDto.ChatId,
                    "Произошла ошибка при обработке вашего сообщения. Пожалуйста, попробуйте позже.",
                    cancellationToken);
            }
            catch (Exception sendEx)
            {
                _logger.LogError(sendEx, "Failed to send error message to chat {ChatId}", messageDto.ChatId);
            }
        }
    }
}

