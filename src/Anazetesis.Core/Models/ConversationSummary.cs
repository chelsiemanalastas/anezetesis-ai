namespace Anazetesis.Core.Models;

/// <summary>Lightweight DTO for the conversations list endpoint.</summary>
public class ConversationSummary
{
    public required Guid Id { get; init; }
    public string? Title { get; init; }
    public required int MessageCount { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}