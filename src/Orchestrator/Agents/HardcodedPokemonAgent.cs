using System.Net.Http.Json;
using Orchestrator.Models;
using Orchestrator.Services;

namespace Orchestrator.Agents;

/// <summary>
/// THIS IS THE CODE THE CANDIDATE TASK ASKS YOU TO REFACTOR.
///
/// It "works", but it is a fixed, hardcoded 3-step pipeline: it only ever knows how to
/// answer questions about Pokemon, it calls PokeAPI directly over HttpClient (no MCP,
/// no tool-calling — the LLM never decides what to call), and every prompt/endpoint is
/// baked into this C# class. Adding a second capability (movies, countries, anything
/// else) today means copy-pasting a whole new class like this one.
///
/// See CANDIDATE_TASK.md at the repo root for what to build instead.
/// </summary>
public class HardcodedPokemonAgent
{
    private readonly HttpClient _pokeApiClient;
    private readonly OllamaCompletionService _ollama;

    public HardcodedPokemonAgent(IHttpClientFactory httpClientFactory, OllamaCompletionService ollama)
    {
        _pokeApiClient = httpClientFactory.CreateClient(nameof(HardcodedPokemonAgent));
        _ollama = ollama;
    }

    public async Task<string> HandleAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        // Step 1 (hardcoded): ask the model to pull a Pokemon name out of free text,
        // via a fixed prompt string, instead of a real tool-calling / entity step.
        var extractPrompt =
            $"""
            Extract only the Pokemon name mentioned in the message below.
            Respond with ONLY the name, lowercase, no punctuation, no extra words.
            If no Pokemon is mentioned, respond with exactly: none

            Message: "{userMessage}"
            """;
        var pokemonName = (await _ollama.CompleteAsync(extractPrompt, cancellationToken))
            .Trim()
            .Trim('"', '.', '\'')
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(pokemonName) || pokemonName == "none")
        {
            return "I can only answer questions about Pokemon right now — try asking about one, e.g. \"tell me about pikachu\".";
        }

        // Step 2 (hardcoded): call the one API this pipeline knows about, directly.
        PokemonDto? pokemon;
        try
        {
            pokemon = await _pokeApiClient.GetFromJsonAsync<PokemonDto>(
                $"pokemon/{Uri.EscapeDataString(pokemonName)}", cancellationToken);
        }
        catch (HttpRequestException)
        {
            pokemon = null;
        }

        if (pokemon is null)
        {
            return $"Sorry, I couldn't find a Pokemon named '{pokemonName}'.";
        }

        // Step 3 (hardcoded): a fixed summarization prompt over the raw API result.
        var summaryPrompt =
            $"""
            Summarize this Pokemon data for a curious user in 2-3 friendly sentences.
            Name: {pokemon.Name}
            Height: {pokemon.Height} (decimetres)
            Weight: {pokemon.Weight} (hectograms)
            Types: {string.Join(", ", pokemon.Types.Select(t => t.Type.Name))}
            Abilities: {string.Join(", ", pokemon.Abilities.Select(a => a.Ability.Name))}
            """;
        return await _ollama.CompleteAsync(summaryPrompt, cancellationToken);
    }
}
