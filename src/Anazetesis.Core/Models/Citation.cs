namespace Anazetesis.Core.Models;

public class Citation
{
    public required string Reference { get; init; }
    public required string Source { get; init; }
    public required string Text { get; init; }
    public int AuthorityLevel { get; init; }
}
