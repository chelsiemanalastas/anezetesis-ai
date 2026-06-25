namespace Anazetesis.Core.Models;

public class Topic
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string Icon { get; init; }
    public required string Description { get; init; }
    public required string Color { get; init; }
    public required IReadOnlyList<string> ExampleQuestions { get; init; }
}
