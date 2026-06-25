namespace Anazetesis.Infrastructure;

using Anazetesis.Core.Interfaces;
using Anazetesis.Infrastructure.OpenRouter;
using Anazetesis.Infrastructure.Topics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind OpenRouter configuration
        services.AddOptions<OpenRouterOptions>()
            .Bind(configuration.GetSection(OpenRouterOptions.SectionName))
            .ValidateOnStart();

        // Read config eagerly to configure the HttpClient base address and auth header
        var openRouterConfig = configuration
            .GetSection(OpenRouterOptions.SectionName)
            .Get<OpenRouterOptions>();

        if (openRouterConfig is null || string.IsNullOrWhiteSpace(openRouterConfig.ApiKey))
            throw new InvalidOperationException(
                $"OpenRouter API key is not configured. " +
                $"Run: dotnet user-secrets set \"{OpenRouterOptions.SectionName}:ApiKey\" \"sk-or-v1-...\" " +
                $"from the Anazetesis.Api project directory.");

        // Typed HttpClient — IHttpClientFactory manages connection pooling and lifetime
        services.AddHttpClient<IAskService, OpenRouterService>(client =>
        {
            // BaseAddress must end with '/' for relative request paths to resolve correctly
            var baseUrl = openRouterConfig.BaseUrl.TrimEnd('/') + '/';
            client.BaseAddress = new Uri(baseUrl);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openRouterConfig.ApiKey}");
            client.DefaultRequestHeaders.Add("HTTP-Referer", "https://anazetesis.ai");
            client.DefaultRequestHeaders.Add("X-Title", "Anazetesis AI");
        });

        // Singleton — pure in-memory data, no I/O
        services.AddSingleton<ITopicsService, TopicsService>();

        return services;
    }
}
