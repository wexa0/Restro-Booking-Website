using System.Text.Json.Serialization;

namespace Resort.Domain.Places;

public class Feature
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string IconKey { get; set; } = string.Empty;

    [JsonIgnore]
    public List<Place> Places { get; set; } = new();

}
