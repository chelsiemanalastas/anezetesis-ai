namespace Anazetesis.Core.Models;

public class AskRequest
{
    public required string Question { get; init; }
    public string? Topic { get; init; }
    public int MaxResults { get; init; } = 5;
}
