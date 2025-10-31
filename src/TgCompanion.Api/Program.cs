using Serilog;
using TgCompanion.Api;
using TgCompanion.Application;
using TgCompanion.Infrastructure;

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/tg-companion-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Telegram Companion Bot");

    var builder = Host.CreateApplicationBuilder(args);

    // Добавляем Serilog
    builder.Services.AddSerilog((services, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/tg-companion-.log", rollingInterval: RollingInterval.Day));

    // Регистрация слоев приложения
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // Worker Service
    builder.Services.AddHostedService<BotWorker>();

    var host = builder.Build();
    await host.RunAsync();
    
    Log.Information("Bot stopped gracefully");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Bot terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

