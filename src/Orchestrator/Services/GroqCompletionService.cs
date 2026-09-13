using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;

namespace Orchestrator.Services;

/// <summary>
/// Thin wrapper around Groq's IChatClient for single-shot text completions.
/// This is deliberately "dumb" (no tools, no config-driven behaviour) — it exists
/// so the hardcoded pipeline in Agents/HardcodedPokemonAgent.cs has something to call.
///
/// Groq exposes an OpenAI-compatible /v1 endpoint, so this uses the official OpenAI
/// SDK pointed at Groq's base URL instead of a Groq-specific client — get a free key
/// at https://console.groq.com/keys.
/// </summary>
public class GroqCompletionService
{
    private readonly IChatClient _chatClient;

    public GroqCompletionService(IConfiguration configuration)
    {
        var baseUrl = configuration["Groq:BaseUrl"] ?? "https://api.groq.com/openai/v1";
        var model = configuration["Groq:Model"] ?? "openai/gpt-oss-120b";
        var apiKey = configuration["Groq:ApiKey"]
            ?? Environment.GetEnvironmentVariable("GROQ_API_KEY")
            ?? throw new InvalidOperationException(
                "No Groq API key configured. Set the GROQ_API_KEY environment variable " +
                "or Groq:ApiKey in configuration (get a free key at https://console.groq.com/keys).");

        var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
        {
            Endpoint = new Uri(baseUrl),
        });
        _chatClient = openAiClient.GetChatClient(model).AsIChatClient();
    }

    public async Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var response = await _chatClient.GetResponseAsync(prompt, cancellationToken: cancellationToken);
        return response.Text.Trim();
    }
}
