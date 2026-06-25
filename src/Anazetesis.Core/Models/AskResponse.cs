namespace Anazetesis.Core.Models;

public class AskResponse
{
    public required string Answer { get; init; }
    public required IReadOnlyList<Citation> Citations { get; init; }
}
