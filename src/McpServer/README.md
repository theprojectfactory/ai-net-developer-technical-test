# McpServer

An MCP (Model Context Protocol) server, built with the official [ModelContextProtocol C# SDK](https://www.nuget.org/packages/ModelContextProtocol), exposing tools over stdio.

## Your task

Pick **one** of the three tool files under `Tools/` and implement it fully:

| File | Public API | API key needed? |
|---|---|---|
| `Tools/PokemonTools.cs` | [pokeapi.co](https://pokeapi.co) | No |
| `Tools/MovieTools.cs` | [themoviedb.org](https://www.themoviedb.org/documentation/api) (TMDB) | Yes — free, read `TMDB_API_KEY` from config/env, never hardcode it |
| `Tools/CountryTools.cs` | [restcountries.com](https://restcountries.com) | No |

Delete (or just ignore) the other two files — an unused `[McpServerToolType]` class with no implemented tools is harmless, but only one API is expected to actually work.

Feel free to add more than one tool method to your chosen class (e.g. a search tool and a details tool) — the starter method is just a minimum.

## Running it standalone

This project uses stdio transport, so it isn't a web server you browse to — it starts, reads JSON-RPC messages from stdin, and writes responses to stdout (all logging goes to stderr so it never corrupts the protocol stream). You can talk to it directly with the [MCP Inspector](https://github.com/modelcontextprotocol/inspector):

```bash
npx @modelcontextprotocol/inspector dotnet run --project src/McpServer/McpServer.csproj
```

That opens a browser UI where you can list and invoke your tools without needing the Orchestrator at all — useful for developing and testing this project in isolation.

## Wiring it into the Orchestrator

See `CANDIDATE_TASK.md` at the repo root — the Orchestrator project needs to launch this project as a subprocess (`StdioClientTransport` with `Command = "dotnet"`, `Arguments = ["run", "--project", "<path to this .csproj>"]`, or point at the built exe) and connect to it as an MCP client so its tools become available to the agent's tool-calling loop.
