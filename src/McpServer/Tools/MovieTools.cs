using System.ComponentModel;
using ModelContextProtocol.Server;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Configuration;

namespace McpServer.Tools;

/// <summary>
/// TODO (candidate task — implement ONLY IF you chose Movies/TV as your API):
/// Expose one or more MCP tools backed by https://www.themoviedb.org (TMDB).
///
/// TMDB requires a free API key (read-only "API Key" from your TMDB account
/// settings). Do NOT hardcode it — read it from configuration/an environment
/// variable (e.g. TMDB_API_KEY) so it never gets committed to source control.
///
/// Suggested first tool: search for a movie or TV show by title and return its
/// overview, release date and rating. Endpoint:
///   GET https://api.themoviedb.org/3/search/movie?query={title}&api_key={key}
///
/// If you did not choose Movies/TV, delete this file (or leave it — an unused
/// [McpServerToolType] class with no tools implemented is harmless).
/// </summary>
[McpServerToolType]
public static class MovieTools
{
    [McpServerTool, Description("Searches TMDB for a movie by title and returns a short summary.")]
    public static Task<string> SearchMovie(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        [Description("The movie or TV show title to search for.")] string title)
    {
        throw new NotImplementedException("TODO: call TMDB (using an API key from configuration) and return a JSON or plain-text summary.");
    }
}
