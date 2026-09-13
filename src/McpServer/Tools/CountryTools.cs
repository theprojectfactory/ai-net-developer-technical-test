using System.ComponentModel;
using ModelContextProtocol.Server;
using Microsoft.Extensions.Http;

namespace McpServer.Tools;

/// <summary>
/// TODO (candidate task — implement ONLY IF you chose Countries as your API):
/// Expose one or more MCP tools backed by https://restcountries.com (no API key required).
///
/// Suggested first tool: given a country name, return its capital, region,
/// population and currencies. Endpoint:
///   GET https://restcountries.com/v3.1/name/{name}
///
/// If you did not choose Countries, delete this file (or leave it — an unused
/// [McpServerToolType] class with no tools implemented is harmless).
/// </summary>
[McpServerToolType]
public static class CountryTools
{
    [McpServerTool, Description("Gets capital, region, population and currencies for a country by name.")]
    public static Task<string> GetCountry(
        IHttpClientFactory httpClientFactory,
        [Description("The country's common name (e.g. 'Australia').")] string name)
    {
        throw new NotImplementedException("TODO: call REST Countries and return a JSON or plain-text summary.");
    }
}
