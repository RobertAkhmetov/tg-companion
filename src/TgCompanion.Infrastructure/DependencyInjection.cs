using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TgCompanion.Domain.Interfaces;
using TgCompanion.Infrastructure.Configuration;
using TgCompanion.Infrastructure.Repositories;
using TgCompanion.Infrastructure.Services;

namespace TgCompanion.Infrastructure;

/// <summary>
/// Регистрация зависимостей Infrastructure слоя
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Конфигурация
        services.Configure<OllamaSettings>(configuration.GetSection(OllamaSettings.SectionName));
        services.Configure<TelegramSettings>(configuration.GetSection(TelegramSettings.SectionName));

        // HTTP клиент для Ollama
        services.AddHttpClient<IAiService, OllamaAiService>();

        // Репозитории
        services.AddSingleton<IConversationRepository, InMemoryConversationRepository>();

        // Сервисы
        services.AddSingleton<ITelegramBotService, TelegramBotService>();

        return services;
    }
}

