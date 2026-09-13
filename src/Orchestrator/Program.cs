using DotNetEnv;
using Orchestrator.Agents;
using Orchestrator.Models;
using Orchestrator.Services;

// Loads GROQ_API_KEY (and anything else) from a .env file into environment variables,
// if one exists anywhere from the current directory up to the repo root. Harmless no-op
// if no .env file is found — falls back to real environment variables / user-secrets.
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient(nameof(HardcodedPokemonAgent), client =>
{
    client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
});

builder.Services.AddSingleton<GroqCompletionService>();
builder.Services.AddScoped<HardcodedPokemonAgent>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/chat", async (ChatRequest request, HardcodedPokemonAgent agent, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Message))
    {
        return Results.BadRequest(new { error = "Message must not be empty." });
    }

    var reply = await agent.HandleAsync(request.Message, cancellationToken);
    return Results.Ok(new ChatResponse(reply));
});

app.Run();
