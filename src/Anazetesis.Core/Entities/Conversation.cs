namespace Anazetesis.Core.Entities;

public class Conversation
{
    public Guid Id { get; set; }
    public required string ClientId { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}