using System.Text.Json.Serialization;

namespace Orchestrator.Models;

// Minimal shape of https://pokeapi.co/api/v2/pokemon/{name} — only the fields the
// hardcoded pipeline below happens to use.
public class PokemonDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonPropertyName("types")]
    public List<PokemonTypeSlot> Types { get; set; } = new();

    [JsonPropertyName("abilities")]
    public List<PokemonAbilitySlot> Abilities { get; set; } = new();
}

public class PokemonTypeSlot
{
    [JsonPropertyName("type")]
    public NamedResource Type { get; set; } = new();
}

public class PokemonAbilitySlot
{
    [JsonPropertyName("ability")]
    public NamedResource Ability { get; set; } = new();
}

public class NamedResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}
