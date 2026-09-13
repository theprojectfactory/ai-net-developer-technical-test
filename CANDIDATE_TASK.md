# Candidate Task

## Context

This repo contains a working AI orchestrator with one built-in capability: it can answer
questions about Pokemon. The way it does that today is deliberately bad practice —
`src/Orchestrator/Agents/HardcodedPokemonAgent.cs` is a fixed, three-step pipeline:

1. Ask the LLM (with a hardcoded prompt string) to extract a Pokemon name from the user's message.
2. Call PokeAPI directly over `HttpClient` (hardcoded endpoint, hardcoded API — no MCP, no tool-calling).
3. Ask the LLM (with another hardcoded prompt string) to summarize the raw API response.

Nothing here is configurable. The model never decides what to call or when — the C# code
does. Adding a second capability today means copy-pasting a whole new class like this one
and wiring up a new hardcoded endpoint.

Separately, `src/McpServer` is a standalone MCP server scaffold with three unimplemented
tool stubs (Pokemon / Movies-TV / Countries — pick one).

## What to build

### 1. Refactor the orchestrator into a generic, configuration-driven agent runtime

Replace the hardcoded pipeline with something that can run **any** agent defined by
configuration, without code changes. Concretely, the runtime should:

- Load an agent definition (system prompt, model name, which MCP server(s)/tools
  it has access to) from configuration — e.g. a JSON file — rather than from C# code.
- Use the LLM's tool-calling support (the model decides which tool to call and with what
  arguments — see [Groq's tool calling docs](https://console.groq.com/docs/tool-use), which
  follow the same OpenAI-compatible `tools` shape as `Microsoft.Extensions.AI`'s `ChatOptions.Tools`)
  instead of fixed prompt strings.
- Drive a real tool-calling loop: send the conversation + available tools to the model,
  execute whatever tool call(s) it requests, feed the result(s) back, repeat until the
  model returns a final answer.
- Keep the same `POST /api/chat` contract (`{ "message": "..." }` → `{ "reply": "..." }`)
  so the existing UI in `wwwroot/index.html` keeps working unmodified.

You do not need to preserve `HardcodedPokemonAgent.cs` itself — replace it.

**Suggested (not mandatory) building blocks**, already referenced in the codebase:

- [`Microsoft.Extensions.AI`](https://learn.microsoft.com/dotnet/ai/ichatclient) — vendor-neutral `IChatClient` / `ChatMessage` / `ChatOptions.Tools` abstractions.
- The OpenAI SDK's `ChatClient`, pointed at Groq's OpenAI-compatible endpoint and wrapped with
  `.AsIChatClient()` — already used in `GroqCompletionService.cs` — implements `IChatClient`.
- `IChatClient` has an extension `.AsBuilder().UseFunctionInvocation().Build()` that turns a
  chat client into one that automatically executes a tool-calling loop for you, given a list
  of `AITool`/`AIFunction` instances in `ChatOptions.Tools`.

You're free to hand-roll the tool-calling loop yourself against Groq's OpenAI-compatible
`/chat/completions` `tools` field instead — both are acceptable, but the config-driven design
matters more than which HTTP plumbing you use to get there.

### 2. Build the MCP server

In `src/McpServer`, implement **one** of the three tool stubs (see `src/McpServer/README.md`
for details and API endpoints):

- **Pokemon** — [pokeapi.co](https://pokeapi.co), no API key.
- **Movies/TV** — [themoviedb.org](https://www.themoviedb.org/documentation/api), free API key required (read from config/env, never commit it).
- **Countries** — [restcountries.com](https://restcountries.com), no API key.

Use the `[McpServerToolType]` / `[McpServerTool]` attributes already in the stub files —
`WithToolsFromAssembly()` in `Program.cs` discovers them automatically. Add a `[Description]`
to every tool and parameter — that description is what the LLM sees when deciding whether
and how to call your tool, so it needs to be genuinely useful, not just a label.

### 3. Wire the MCP server into the orchestrator

The generic agent runtime from step 1 should get its tools from the MCP server in step 2 —
not from a hardcoded `HttpClient` call. Concretely:

- Connect to `src/McpServer` as an MCP client from the Orchestrator (stdio transport is the
  simplest option — the Orchestrator process launches the MCP server as a subprocess).
- List the MCP server's tools and make them available to the model in the tool-calling loop
  from step 1 (an `McpClientTool` from `ListToolsAsync()` can be used directly as an
  `AITool`/`AIFunction` if you went the `Microsoft.Extensions.AI` route).
- The agent's system prompt/config from step 1 should reflect the domain you chose in step 2
  (e.g. if you implemented Countries, the agent should be a "countries" agent, not still
  claim to be about Pokemon).

### 4. Verify end to end

Run the Orchestrator, open the UI, and ask a question in your chosen domain (e.g. "what's
the capital of Japan?" for Countries). Confirm in the logs (or by adding your own logging)
that the model actually issued a tool call to your MCP server, rather than answering from
its own training data.

## Stretch goals (optional)

- Support more than one agent/tool-set being selectable at runtime without code changes.
- Add a second MCP server, or a second tool to your chosen one.
- Basic error handling for tool failures (e.g. API is down, entity not found) that the
  agent surfaces sensibly to the user instead of crashing.

## What we're evaluating

- Whether the orchestrator refactor is genuinely configuration-driven (could a new agent be
  added by editing config alone, with zero changes to the core runtime code?).
- Correctness of the MCP integration (protocol usage, tool discovery, tool-calling loop).
- Code quality and structure of the new MCP server tool(s).
- Whether the API key handling (if you chose Movies/TV) avoids hardcoding secrets.
- Whether the existing UI still works, unmodified, against your refactored backend.
- Clear commit history / a short note on any tradeoffs or things you'd do with more time.
