namespace Anazetesis.Infrastructure.OpenRouter;

using System.Text.Json.Serialization;

// ── HTTP Request DTOs ─────────────────────────────────────────────────────────

internal record ChatRequest(
    [property: JsonPropertyName("model")]           string Model,
    [property: JsonPropertyName("messages")]        IReadOnlyList<ChatMessage> Messages,
    [property: JsonPropertyName("max_tokens")]      int MaxTokens,
    [property: JsonPropertyName("temperature")]     double Temperature = 0.3,
    [property: JsonPropertyName("response_format")] ResponseFormat? ResponseFormat = null
);

internal record ChatMessage(
    [property: JsonPropertyName("role")]    string Role,
    [property: JsonPropertyName("content")] string Content
);

internal record ResponseFormat(
    [property: JsonPropertyName("type")] string Type
);

// ── HTTP Response DTOs ────────────────────────────────────────────────────────

internal record ChatResponse(
    [property: JsonPropertyName("choices")] IReadOnlyList<ChatChoice> Choices
);

internal record ChatChoice(
    [property: JsonPropertyName("message")] ChatMessage Message
);

// ── Structured LLM output (what the AI returns as JSON inside content) ────────

internal record LlmStructuredResponse(
    [property: JsonPropertyName("answer")]    string Answer,
    [property: JsonPropertyName("citations")] IReadOnlyList<LlmCitation> Citations
);

internal record LlmCitation(
    [property: JsonPropertyName("reference")]      string Reference,
    [property: JsonPropertyName("source")]         string Source,
    [property: JsonPropertyName("text")]           string Text,
    [property: JsonPropertyName("authorityLevel")] int AuthorityLevel
);
