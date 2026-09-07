# .NET AI Developer — Technical Test Template

A working starter solution for a take-home technical test focused on two things:

1. Refactoring a **hardcoded agentic workflow** into a **generic, configuration-driven agent orchestrator**.
2. Building a **.NET MCP (Model Context Protocol) server** and wiring it into that orchestrator as its source of tools.

Everything below "Prerequisites" and "Run it" already works out of the box — that's the baseline you're refactoring, not the target. The actual assignment is in [CANDIDATE_TASK.md](CANDIDATE_TASK.md).

## Solution layout

```
TechnicalTestTemplate.sln
src/
  Orchestrator/     ASP.NET Core app: hardcoded agent pipeline + /api/chat endpoint + static UI (wwwroot/index.html)
  McpServer/         MCP server scaffold (stdio transport) — three TODO tool stubs, implement one
```

## Prerequisites

- .NET 8 SDK
- [Ollama](https://ollama.com) installed and running locally (`ollama serve`, or the desktop app)
- A tool-calling-capable model pulled, e.g.:
  ```bash
  ollama pull llama3.1:8b
  ```
  (Any model whose `ollama show <model>` lists `tools` under capabilities will work. Update `Ollama:Model` in `src/Orchestrator/appsettings.json` if you use a different one.)

## Run it

```bash
dotnet run --project src/Orchestrator/Orchestrator.csproj
```

This launches a browser at the app's URL (see the console output, typically `https://localhost:7139`) showing a small chat UI. Try:

> tell me about pikachu

The request flows: UI → `POST /api/chat` → a hardcoded 3-step pipeline (`src/Orchestrator/Agents/HardcodedPokemonAgent.cs`) → PokeAPI (direct HTTP) + Ollama (direct prompts, no tool-calling) → a reply back in the UI.

That pipeline is intentionally rigid — it's the thing described in [CANDIDATE_TASK.md](CANDIDATE_TASK.md) that needs to become generic and MCP-driven.

## The MCP server

`src/McpServer` is a separate, standalone .NET project — a scaffold, not a finished server. It builds and runs as-is, but its three tool classes under `Tools/` all currently throw `NotImplementedException`. See `src/McpServer/README.md` for what to build there.

## Configuration

`src/Orchestrator/appsettings.json`:

```json
"Ollama": {
  "BaseUrl": "http://localhost:11434",
  "Model": "llama3.1:8b"
}
```

If you implement the Movies/TV tool (TMDB), you'll need a free TMDB API key — read it from configuration or an environment variable in your MCP server code; do not commit it.
