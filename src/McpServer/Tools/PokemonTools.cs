using System.ComponentModel;
using ModelContextProtocol.Server;
using Microsoft.Extensions.Http;

namespace McpServer.Tools;

/// <summary>
/// TODO (candidate task — implement ONLY IF you chose Pokemon as your API):
/// Expose one or more MCP tools backed by https://pokeapi.co (no API key required).
///
/// Suggested first tool: given a Pokemon name or id, return its height, weight,
/// types and abilities — the same fields the hardcoded orchestrator pipeline
/// currently fetches by calling PokeAPI directly. Endpoint:
///   GET https://pokeapi.co/api/v2/pokemon/{name-or-id}
///
/// If you did not choose Pokemon, delete this file (or leave it — an unused
/// [McpServerToolType] class with no tools implemented is harmless).
/// </summary>
[McpServerToolType]
public static class PokemonTools
{
    [McpServerTool, Description("Gets height, weight, types and abilities for a Pokemon by name or id.")]
    public static Task<string> GetPokemon(
        IHttpClientFactory httpClientFactory,
        [Description("The Pokemon's name (e.g. 'pikachu') or numeric id.")] string nameOrId)
    {
        throw new NotImplementedException("TODO: call PokeAPI and return a JSON or plain-text summary.");
    }
}
