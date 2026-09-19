using Anazetesis.Core.Entities;

namespace Anazetesis.Core.Interfaces;

public interface IChatRepository
{
    Task<Conversation?> GetConversationAsync(Guid conversationId, CancellationToken ct = default);
    Task<IReadOnlyList<Conversation>> GetConversationsAsync(string clientId, int count = 20, CancellationToken ct = default);
    Task<Conversation> CreateConversationAsync(string clientId, string? title, CancellationToken ct = default);
    Task AddMessageAsync(ChatMessage message, CancellationToken ct = default);
    Task<IReadOnlyList<ChatMessage>> GetRecentMessagesAsync(Guid conversationId, int count = 20, CancellationToken ct = default);
    Task UpdateConversationAsync(CancellationToken ct = default);
}