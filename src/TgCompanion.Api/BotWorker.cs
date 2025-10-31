using TgCompanion.Domain.Interfaces;

namespace TgCompanion.Api;

/// <summary>
/// Background Worker для запуска Telegram бота
/// </summary>
public sealed class BotWorker : BackgroundService
{
    private readonly ITelegramBotService _telegramBotService;
    private readonly IAiService _aiService;
    private readonly ILogger<BotWorker> _logger;

    public BotWorker(
        ITelegramBotService telegramBotService,
        IAiService aiService,
        ILogger<BotWorker> logger)
    {
        _telegramBotService = telegramBotService;
        _aiService = aiService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Bot Worker starting...");

        // Проверяем доступность Ollama
        var ollamaAvailable = await _aiService.IsAvailableAsync(stoppingToken);
        if (!ollamaAvailable)
        {
            _logger.LogWarning(
                "Ollama service is not available. " +
                "Make sure Ollama is running and the model is pulled. " +
                "Bot will continue, but AI responses will fail.");
        }
        else
        {
            _logger.LogInformation("Ollama service is available");
        }

        // Запускаем бота
        await _telegramBotService.StartReceivingAsync(stoppingToken);

        // Ждем сигнала остановки
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bot Worker stopping...");
        
        await _telegramBotService.StopReceivingAsync(cancellationToken);
        
        await base.StopAsync(cancellationToken);
        
        _logger.LogInformation("Bot Worker stopped");
    }
}

