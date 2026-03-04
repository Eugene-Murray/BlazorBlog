using System.Text.Json.Serialization;

namespace BlazorBlog.Models.AlphaVantage;

public class TopGainersLosersResponse
{
    [JsonPropertyName("metadata")]
    public string? Metadata { get; set; }

    [JsonPropertyName("last_updated")]
    public string? LastUpdated { get; set; }

    [JsonPropertyName("top_gainers")]
    public List<MarketMover>? TopGainers { get; set; }

    [JsonPropertyName("top_losers")]
    public List<MarketMover>? TopLosers { get; set; }

    [JsonPropertyName("most_actively_traded")]
    public List<MarketMover>? MostActivelyTraded { get; set; }
}

public class MarketMover
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public string Price { get; set; } = string.Empty;

    [JsonPropertyName("change_amount")]
    public string ChangeAmount { get; set; } = string.Empty;

    [JsonPropertyName("change_percentage")]
    public string ChangePercentage { get; set; } = string.Empty;

    [JsonPropertyName("volume")]
    public string Volume { get; set; } = string.Empty;

    public decimal PriceValue => decimal.TryParse(Price, out var val) ? val : 0;
    public decimal ChangeAmountValue => decimal.TryParse(ChangeAmount, out var val) ? val : 0;
    public decimal ChangePercentageValue => decimal.TryParse(ChangePercentage.TrimEnd('%'), out var val) ? val : 0;
    public long VolumeValue => long.TryParse(Volume, out var val) ? val : 0;
}
