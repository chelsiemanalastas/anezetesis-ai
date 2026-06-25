namespace Anazetesis.Infrastructure.OpenRouter;

public class OpenRouterOptions
{
    public const string SectionName = "OpenRouter";

    public string ApiKey { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = "https://openrouter.ai/api/v1";
    public string Model { get; init; } = "anthropic/claude-sonnet-4-5";
    public int MaxTokens { get; init; } = 2048;
}
