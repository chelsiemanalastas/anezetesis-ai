namespace Anazetesis.Infrastructure.OpenRouter;

using System.Net.Http.Json;
using System.Text.Json;
using Anazetesis.Core.Interfaces;
using Anazetesis.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class OpenRouterService : IAskService
{
    private readonly HttpClient _http;
    private readonly OpenRouterOptions _options;
    private readonly ILogger<OpenRouterService> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OpenRouterService(
        HttpClient http,
        IOptions<OpenRouterOptions> options,
        ILogger<OpenRouterService> logger)
    {
        _http    = http;
        _options = options.Value;
        _logger  = logger;
    }

    public async Task<AskResponse> AskAsync(AskRequest request, CancellationToken ct = default)
    {
        var chatRequest = new ChatRequest(
            Model:          _options.Model,
            Messages:       new List<ChatMessage>
            {
                new("system", BuildSystemPrompt(request.Topic)),
                new("user",   BuildUserPrompt(request.Question, request.MaxResults))
            },
            MaxTokens:      _options.MaxTokens,
            Temperature:    0.3,
            ResponseFormat: new ResponseFormat("json_object")
        );

        _logger.LogInformation("Calling OpenRouter [{Model}]: {Question}",
            _options.Model, request.Question);

        var httpResponse = await _http.PostAsJsonAsync("chat/completions", chatRequest, JsonOpts, ct);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var body = await httpResponse.Content.ReadAsStringAsync(ct);
            _logger.LogError("OpenRouter {Status}: {Body}", (int)httpResponse.StatusCode, body);
            throw new HttpRequestException(
                $"OpenRouter API error {(int)httpResponse.StatusCode}: {body}");
        }

        var chatResponse = await httpResponse.Content
            .ReadFromJsonAsync<ChatResponse>(JsonOpts, ct)
            ?? throw new InvalidOperationException("Empty response from OpenRouter.");

        var rawContent = chatResponse.Choices[0].Message.Content;

        LlmStructuredResponse structured;
        try
        {
            structured = JsonSerializer.Deserialize<LlmStructuredResponse>(rawContent, JsonOpts)
                ?? throw new JsonException("Null deserialization result.");
        }
        catch (JsonException ex)
        {
            // Graceful degradation: LLM ignored the JSON instruction (rare with Claude)
            _logger.LogWarning(ex, "LLM returned non-JSON content. Falling back to plain text response.");
            return new AskResponse
            {
                Answer    = rawContent,
                Citations = new List<Citation>().AsReadOnly()
            };
        }

        var citations = structured.Citations
            .Select(c => new Citation
            {
                Reference      = c.Reference,
                Source         = c.Source,
                Text           = c.Text,
                AuthorityLevel = c.AuthorityLevel
            })
            .ToList()
            .AsReadOnly();

        return new AskResponse
        {
            Answer    = structured.Answer,
            Citations = citations
        };
    }

    private static string BuildSystemPrompt(string? topic)
    {
        var topicFocus = topic switch
        {
            "ten_commandments" => "Focus on the Ten Commandments and the moral law as taught by the Church.",
            "core_beliefs"     => "Focus on core doctrines: the Trinity, Incarnation, Resurrection, and articles of the Creed.",
            "sacraments"       => "Focus on the seven sacraments, their biblical basis, and theological meaning.",
            "scripture"        => "Focus on Sacred Scripture, biblical translation, and Catholic exegesis.",
            "tradition"        => "Focus on Sacred Tradition, liturgical development, and Catholic culture.",
            "apologetics"      => "Focus on rational defense of Catholic teachings and arguments for the faith.",
            "defending_faith"  => "Focus on responses to modern challenges: atheism, scientism, and relativism.",
            _                  => "Answer any question about Catholic theology and tradition."
        };

        return $$"""
            You are Anazetesis, an expert in Catholic theology with deep knowledge of:
            - Catechism of the Catholic Church (CCC)
            - Sacred Scripture (Douay-Rheims and RSV-CE translations)
            - Church Fathers: Ignatius of Antioch, Justin Martyr, Irenaeus, Tertullian, Athanasius,
              Augustine, Jerome, John Chrysostom, Leo the Great, Gregory the Great, and others
            - Ecumenical Councils: Nicaea I, Constantinople I, Ephesus, Chalcedon, Trent, Vatican I & II
            - Papal encyclicals and apostolic exhortations
            - Summa Theologiae by Thomas Aquinas

            {{topicFocus}}

            CRITICAL INSTRUCTION: Your entire response must be ONLY a raw JSON object. No text before it, no text after it, no markdown code fences (no ```), no "Here is..." preamble. Start your response with { and end with }.

            The JSON object must have exactly this shape:
            {"answer":"<markdown text here>","citations":[{"reference":"<ref>","source":"<source>","text":"<quote>","authorityLevel":<number>}]}

            For the answer field:
            - Write in Markdown: use ## for section headings, **bold** for key terms, *italic* for direct quotations from sources
            - Be thorough and well-structured with multiple paragraphs
            - Escape all double quotes inside the answer string as \"

            For citations (include 3-6):
            - reference: e.g. "CCC 1324" or "John 6:53" or "Augustine, Confessions 1.1" or "Council of Trent, Session 7"
            - source: one of these exact values: catechism, scripture, church_father, council, summa, encyclical
            - text: the exact quoted passage, 1-3 sentences, escaped for JSON
            - authorityLevel: integer 1-5 (5=CCC/Scripture/Council, 4=Church Father, 3=Summa, 2=Encyclical, 1=Other)

            Never fabricate quotes. Maintain a reverent, scholarly tone.
            """;
    }

    private static string BuildUserPrompt(string question, int maxResults) =>
        $"Please answer this Catholic theology question. Provide at most {maxResults} citations.\n\nQuestion: {question}";
}
