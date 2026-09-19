namespace Anazetesis.Core.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public required string Role { get; set; }          // "user", "assistant", "system"
    public required string Content { get; set; }
    public int? TokensUsed { get; set; }
    public DateTime CreatedAt { get; set; }

    public Conversation Conversation { get; set; } = null!;
}