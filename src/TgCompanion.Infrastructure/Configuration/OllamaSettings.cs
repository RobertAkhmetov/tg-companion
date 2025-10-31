namespace TgCompanion.Infrastructure.Configuration;

/// <summary>
/// Настройки Ollama
/// </summary>
public sealed class OllamaSettings
{
    public const string SectionName = "Ollama";

    /// <summary>
    /// Базовый URL Ollama API
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:11434";

    /// <summary>
    /// Название модели для использования
    /// </summary>
    public string Model { get; set; } = "deepseek-r1:8b";

    /// <summary>
    /// Температура генерации (0.0 - 1.0)
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Максимальное количество токенов в ответе
    /// </summary>
    public int MaxTokens { get; set; } = 2000;

    /// <summary>
    /// Системный промпт
    /// </summary>
    public string SystemPrompt { get; set; } = 
        "Ты - дружелюбный и полезный AI-ассистент в Telegram. " +
        "Отвечай на вопросы пользователей кратко, понятно и по-дружески. " +
        "Используй русский язык, если пользователь пишет на русском.";

    /// <summary>
    /// Таймаут запроса в секундах
    /// </summary>
    public int TimeoutSeconds { get; set; } = 120;
}

