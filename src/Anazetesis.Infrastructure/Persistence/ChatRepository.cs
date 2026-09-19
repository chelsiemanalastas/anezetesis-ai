using Anazetesis.Core.Entities;
using Anazetesis.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anazetesis.Infrastructure.Persistence;

public sealed class ChatRepository : IChatRepository
{
    private readonly AppDbContext _db;

    public ChatRepository(AppDbContext db) => _db = db;

    public async Task<Conversation?> GetConversationAsync(Guid conversationId, CancellationToken ct = default) =>
        await _db.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId, ct);

    public async Task<IReadOnlyList<Conversation>> GetConversationsAsync(
        string clientId, int count = 20, CancellationToken ct = default) =>
        await _db.Conversations
            .AsNoTracking()
            .Include(c => c.Messages)   // Required: ConversationSummary.MessageCount reads c.Messages.Count
            .Where(c => c.ClientId == clientId)
            .OrderByDescending(c => c.UpdatedAt)
            .Take(count)
            .ToListAsync(ct);

    public async Task<Conversation> CreateConversationAsync(
        string clientId, string? title, CancellationToken ct = default)
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            Title = title,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Conversations.Add(conversation);
        await _db.SaveChangesAsync(ct);

        return conversation;
    }

    public async Task AddMessageAsync(ChatMessage message, CancellationToken ct = default)
    {
        _db.ChatMessages.Add(message);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ChatMessage>> GetRecentMessagesAsync(
        Guid conversationId, int count = 20, CancellationToken ct = default) =>
        await _db.ChatMessages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(count)
            .OrderBy(m => m.CreatedAt) // Re-sort ascending for the LLM
            .ToListAsync(ct);

    public async Task UpdateConversationAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }
}