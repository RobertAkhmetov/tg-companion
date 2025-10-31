using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgCompanion.Application.DTOs;
using TgCompanion.Application.UseCases;
using TgCompanion.Domain.Interfaces;
using TgCompanion.Infrastructure.Configuration;

namespace TgCompanion.Infrastructure.Services;

/// <summary>
/// Реализация сервиса Telegram бота
/// </summary>
public sealed class TelegramBotService : ITelegramBotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ProcessMessageUseCase _processMessageUseCase;
    private readonly ClearConversationUseCase _clearConversationUseCase;
    private readonly ILogger<TelegramBotService> _logger;
    private CancellationTokenSource? _receivingCts;

    public TelegramBotService(
        IOptions<TelegramSettings> settings,
        ProcessMessageUseCase processMessageUseCase,
        ClearConversationUseCase clearConversationUseCase,
        ILogger<TelegramBotService> logger)
    {
        var token = settings.Value.BotToken;
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Telegram bot token is not configured");
        }

        _botClient = new TelegramBotClient(token);
        _processMessageUseCase = processMessageUseCase;
        _clearConversationUseCase = clearConversationUseCase;
        _logger = logger;
    }

    public async Task SendTextMessageAsync(
        long chatId,
        string text,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _botClient.SendMessage(
                chatId: chatId,
                text: text,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to chat {ChatId}", chatId);
            throw;
        }
    }

    public async Task SendTypingActionAsync(
        long chatId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _botClient.SendChatAction(
                chatId: chatId,
                action: ChatAction.Typing,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error sending typing action to chat {ChatId}", chatId);
        }
    }

    public async Task StartReceivingAsync(CancellationToken cancellationToken = default)
    {
        _receivingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message },
            DropPendingUpdates = true
        };

        var me = await _botClient.GetMe(cancellationToken);
        _logger.LogInformation("Starting bot @{BotUsername}", me.Username);

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: _receivingCts.Token);

        _logger.LogInformation("Bot started successfully");
    }

    public Task StopReceivingAsync(CancellationToken cancellationToken = default)
    {
        _receivingCts?.Cancel();
        _receivingCts?.Dispose();
        _receivingCts = null;

        _logger.LogInformation("Bot stopped");
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;

        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        var userId = message.From?.Id ?? 0;
        var userName = message.From?.Username ?? "Unknown";

        _logger.LogInformation(
            "Received message from {UserName} ({UserId}) in chat {ChatId}: {MessageText}",
            userName, userId, chatId, messageText);

        // Обработка команд
        if (messageText.StartsWith('/'))
        {
            await HandleCommandAsync(chatId, messageText, cancellationToken);
            return;
        }

        // Обработка обычного сообщения
        var messageDto = new TelegramMessageDto(
            ChatId: chatId,
            MessageId: message.MessageId,
            UserId: userId,
            UserName: userName,
            FirstName: message.From?.FirstName,
            LastName: message.From?.LastName,
            Text: messageText,
            ReceivedAt: DateTime.UtcNow);

        await _processMessageUseCase.ExecuteAsync(messageDto, cancellationToken);
    }

    private async Task HandleCommandAsync(
        long chatId,
        string command,
        CancellationToken cancellationToken)
    {
        var commandText = command.Split(' ')[0].ToLowerInvariant();

        switch (commandText)
        {
            case "/start":
                await SendTextMessageAsync(
                    chatId,
                    "👋 Привет! Я AI-ассистент на базе DeepSeek-R1.\n\n" +
                    "Я могу поддержать разговор, ответить на вопросы и помочь с различными задачами.\n\n" +
                    "Доступные команды:\n" +
                    "/help - показать справку\n" +
                    "/clear - очистить историю разговора\n" +
                    "/about - информация о боте",
                    cancellationToken);
                break;

            case "/help":
                await SendTextMessageAsync(
                    chatId,
                    "📖 Справка:\n\n" +
                    "Просто напишите мне любое сообщение, и я постараюсь помочь!\n\n" +
                    "Команды:\n" +
                    "/start - начать работу\n" +
                    "/clear - очистить историю разговора\n" +
                    "/about - информация о боте\n" +
                    "/help - эта справка",
                    cancellationToken);
                break;

            case "/clear":
                await _clearConversationUseCase.ExecuteAsync(chatId, cancellationToken);
                break;

            case "/about":
                await SendTextMessageAsync(
                    chatId,
                    "🤖 О боте:\n\n" +
                    "Telegram AI Companion\n" +
                    "Модель: DeepSeek-R1 (через Ollama)\n" +
                    "Архитектура: Clean Architecture на .NET 9\n\n" +
                    "Создан для демонстрации интеграции современных технологий.",
                    cancellationToken);
                break;

            default:
                await SendTextMessageAsync(
                    chatId,
                    "❓ Неизвестная команда. Используйте /help для списка команд.",
                    cancellationToken);
                break;
        }
    }

    private Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error occurred while receiving updates");
        return Task.CompletedTask;
    }
}

