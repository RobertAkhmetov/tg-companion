using Microsoft.Extensions.DependencyInjection;
using TgCompanion.Application.UseCases;

namespace TgCompanion.Application;

/// <summary>
/// Регистрация зависимостей Application слоя
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Регистрируем use cases
        services.AddScoped<ProcessMessageUseCase>();
        services.AddScoped<ClearConversationUseCase>();

        return services;
    }
}

