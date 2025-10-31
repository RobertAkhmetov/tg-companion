using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using TgCompanion.Domain.Entities;
using TgCompanion.Domain.Interfaces;

namespace TgCompanion.Infrastructure.Repositories;

/// <summary>
/// In-memory реализация репозитория разговоров
/// </summary>
public sealed class InMemoryConversationRepository : IConversationRepository
{
    private readonly ConcurrentDictionary<long, Conversation> _conversations = new();
    private readonly ILogger<InMemoryConversationRepository> _logger;

    public InMemoryConversationRepository(ILogger<InMemoryConversationRepository> logger)
    {
        _logger = logger;
    }

    public Task<Conversation?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        _conversations.TryGetValue(chatId, out var conversation);
        
        if (conversation != null)
        {
            _logger.LogDebug("Retrieved conversation for chat {ChatId} with {MessageCount} messages",
                chatId, conversation.Messages.Count);
        }
        
        return Task.FromResult(conversation);
    }

    public Task SaveAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        _conversations.AddOrUpdate(
            conversation.ChatId,
            conversation,
            (_, _) => conversation);

        _logger.LogDebug("Saved conversation for chat {ChatId} with {MessageCount} messages",
            conversation.ChatId, conversation.Messages.Count);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(long chatId, CancellationToken cancellationToken = default)
    {
        _conversations.TryRemove(chatId, out _);
        
        _logger.LogDebug("Deleted conversation for chat {ChatId}", chatId);
        
        return Task.CompletedTask;
    }
}

