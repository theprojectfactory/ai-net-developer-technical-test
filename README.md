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
- A free [Groq](https://console.groq.com) account and API key ([console.groq.com/keys](https://console.groq.com/keys)) — Groq's free tier serves tool-calling-capable models (e.g. `openai/gpt-oss-120b`) over an OpenAI-compatible endpoint, so no local model runtime is required. Run `curl https://api.groq.com/openai/v1/models -H "Authorization: Bearer $GROQ_API_KEY"` any time to see what's currently live — Groq retires/renames models fairly often.
- Set the key as an environment variable before running:
  ```bash
  export GROQ_API_KEY=gsk_...          # macOS/Linux
  $env:GROQ_API_KEY = "gsk_..."        # PowerShell
  ```
  (Update `Groq:Model` in `src/Orchestrator/appsettings.json` if you want a different Groq model — see the current list at [console.groq.com/docs/models](https://console.groq.com/docs/models).)

## Run it

```bash
dotnet run --project src/Orchestrator/Orchestrator.csproj
```

This launches a browser at the app's URL (see the console output, typically `https://localhost:7139`) showing a small chat UI. Try:

> tell me about pikachu

The request flows: UI → `POST /api/chat` → a hardcoded 3-step pipeline (`src/Orchestrator/Agents/HardcodedPokemonAgent.cs`) → PokeAPI (direct HTTP) + Groq (direct prompts, no tool-calling) → a reply back in the UI.

That pipeline is intentionally rigid — it's the thing described in [CANDIDATE_TASK.md](CANDIDATE_TASK.md) that needs to become generic and MCP-driven.

## The MCP server

`src/McpServer` is a separate, standalone .NET project — a scaffold, not a finished server. It builds and runs as-is, but its three tool classes under `Tools/` all currently throw `NotImplementedException`. See `src/McpServer/README.md` for what to build there.

## Configuration

`src/Orchestrator/appsettings.json`:

```json
"Groq": {
  "BaseUrl": "https://api.groq.com/openai/v1",
  "Model": "llama-3.3-70b-versatile"
}
```

The API key itself is never read from this file — `GroqCompletionService` reads `Groq:ApiKey` from configuration (e.g. user secrets) if set, otherwise falls back to the `GROQ_API_KEY` environment variable. Either way, don't commit a real key.

If you implement the Movies/TV tool (TMDB), you'll need a free TMDB API key — read it from configuration or an environment variable in your MCP server code; do not commit it.
