using Microsoft.Extensions.AI;

namespace Orchestrator.Services;

/// <summary>
/// Thin wrapper around the Ollama IChatClient for single-shot text completions.
/// This is deliberately "dumb" (no tools, no config-driven behaviour) — it exists
/// so the hardcoded pipeline in Agents/HardcodedPokemonAgent.cs has something to call.
/// </summary>
public class OllamaCompletionService
{
    private readonly IChatClient _chatClient;

    public OllamaCompletionService(IConfiguration configuration)
    {
        var baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
        var model = configuration["Ollama:Model"] ?? "llama3.1:8b";
        _chatClient = new OllamaSharp.OllamaApiClient(new Uri(baseUrl), model);
    }

    public async Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var response = await _chatClient.GetResponseAsync(prompt, cancellationToken: cancellationToken);
        return response.Text.Trim();
    }
}
