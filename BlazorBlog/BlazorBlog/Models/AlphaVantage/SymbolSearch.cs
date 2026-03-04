using System.Text.Json.Serialization;

namespace BlazorBlog.Models.AlphaVantage;

public class SymbolSearchResponse
{
    [JsonPropertyName("bestMatches")]
    public List<SymbolMatch>? BestMatches { get; set; }
}

public class SymbolMatch
{
    [JsonPropertyName("1. symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("2. name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("3. type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("4. region")]
    public string Region { get; set; } = string.Empty;

    [JsonPropertyName("5. marketOpen")]
    public string MarketOpen { get; set; } = string.Empty;

    [JsonPropertyName("6. marketClose")]
    public string MarketClose { get; set; } = string.Empty;

    [JsonPropertyName("7. timezone")]
    public string Timezone { get; set; } = string.Empty;

    [JsonPropertyName("8. currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("9. matchScore")]
    public string MatchScore { get; set; } = string.Empty;
}
