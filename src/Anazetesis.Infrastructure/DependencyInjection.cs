namespace Anazetesis.Infrastructure;

using Anazetesis.Core.Interfaces;
using Anazetesis.Infrastructure.OpenRouter;
using Anazetesis.Infrastructure.Persistence;
using Anazetesis.Infrastructure.Topics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class DependencyInjection
{
    private const string DefaultConnectionName = "Default";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddOpenRouter(services, configuration);
        AddPersistence(services, configuration);

        // Singleton — pure in-memory data, no I/O
        services.AddSingleton<ITopicsService, TopicsService>();

        return services;
    }

    private static void AddOpenRouter(IServiceCollection services, IConfiguration configuration)
    {
        // Bind OpenRouter configuration
        services.AddOptions<OpenRouterOptions>()
            .Bind(configuration.GetSection(OpenRouterOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey),
                $"OpenRouter API key is not configured. " +
                $"Run: dotnet user-secrets set \"{OpenRouterOptions.SectionName}:ApiKey\" \"sk-or-v1-...\" " +
                $"from the Anazetesis.Api project directory.")
            .Validate(o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _),
                $"\"{OpenRouterOptions.SectionName}:BaseUrl\" must be an absolute URI, " +
                $"e.g. https://openrouter.ai/api/v1.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Model),
                $"\"{OpenRouterOptions.SectionName}:Model\" must be set, " +
                $"e.g. anthropic/claude-sonnet-4-5.")
            .ValidateOnStart();

        // Typed HttpClient — IHttpClientFactory manages connection pooling and lifetime.
        // Options are resolved per-handler rather than read eagerly, so ValidateOnStart
        // surfaces configuration problems as a clear startup failure instead of a UriFormatException.
        services.AddHttpClient<IAskService, OpenRouterService>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<OpenRouterOptions>>().Value;

            // BaseAddress must end with '/' for relative request paths to resolve correctly
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + '/');

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
            client.DefaultRequestHeaders.Add("HTTP-Referer", "https://anazetesis.ai");
            client.DefaultRequestHeaders.Add("X-Title", "Anazetesis AI");
        });
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(DefaultConnectionName);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                $"Connection string \"{DefaultConnectionName}\" is not configured. " +
                $"Add it under \"ConnectionStrings:{DefaultConnectionName}\" in appsettings.json.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        // Scoped — matches the DbContext lifetime it depends on
        services.AddScoped<IChatRepository, ChatRepository>();
    }
}
