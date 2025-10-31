using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TgCompanion.Domain.Entities;
using TgCompanion.Domain.Interfaces;
using TgCompanion.Infrastructure.Configuration;

namespace TgCompanion.Infrastructure.Services;

/// <summary>
/// Реализация AI сервиса через Ollama
/// </summary>
public sealed class OllamaAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaSettings _settings;
    private readonly ILogger<OllamaAiService> _logger;

    public OllamaAiService(
        HttpClient httpClient,
        IOptions<OllamaSettings> settings,
        ILogger<OllamaAiService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    public async Task<string> GenerateResponseAsync(
        IEnumerable<ChatMessage> conversationHistory,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var messages = BuildMessages(conversationHistory);
            
            var request = new
            {
                model = _settings.Model,
                messages = messages,
                stream = false,
                options = new
                {
                    temperature = _settings.Temperature,
                    num_predict = _settings.MaxTokens
                }
            };

            _logger.LogDebug("Sending request to Ollama: {Model}", _settings.Model);

            var response = await _httpClient.PostAsJsonAsync("/api/chat", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken);
            
            var content = result?.Message?.Content ?? string.Empty;
            
            _logger.LogDebug("Received response from Ollama: {Length} characters", content.Length);

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating response from Ollama");
            throw;
        }
    }

    public async IAsyncEnumerable<string> GenerateResponseStreamAsync(
        IEnumerable<ChatMessage> conversationHistory,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var messages = BuildMessages(conversationHistory);
        
        var request = new
        {
            model = _settings.Model,
            messages = messages,
            stream = true,
            options = new
            {
                temperature = _settings.Temperature,
                num_predict = _settings.MaxTokens
            }
        };

        var content = JsonContent.Create(request);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
        {
            Content = content
        };

        using var response = await _httpClient.SendAsync(
            httpRequest, 
            HttpCompletionOption.ResponseHeadersRead, 
            cancellationToken);
        
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            
            if (string.IsNullOrWhiteSpace(line))
                continue;

            OllamaChatResponse? streamResponse;
            try
            {
                streamResponse = JsonSerializer.Deserialize<OllamaChatResponse>(line);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse streaming response line: {Line}", line);
                continue;
            }

            if (streamResponse?.Message?.Content is { } chunk)
            {
                yield return chunk;
            }

            if (streamResponse?.Done == true)
            {
                break;
            }
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ollama service is not available");
            return false;
        }
    }

    private List<object> BuildMessages(IEnumerable<ChatMessage> conversationHistory)
    {
        var messages = new List<object>
        {
            new { role = "system", content = _settings.SystemPrompt }
        };

        foreach (var msg in conversationHistory)
        {
            var role = msg.Role switch
            {
                MessageRole.User => "user",
                MessageRole.Assistant => "assistant",
                MessageRole.System => "system",
                _ => "user"
            };

            messages.Add(new { role, content = msg.Text });
        }

        return messages;
    }

    private sealed record OllamaChatResponse(
        OllamaMessage? Message,
        bool Done);

    private sealed record OllamaMessage(string? Content);
}

