using System.Text.Json.Serialization;

namespace BlazorBlog.Models.AlphaVantage;

public class StockQuote
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Price { get; set; }
    public long Volume { get; set; }
    public string LatestTradingDay { get; set; } = string.Empty;
    public decimal PreviousClose { get; set; }
    public decimal Change { get; set; }
    public string ChangePercent { get; set; } = string.Empty;

    public decimal ChangePercentValue => decimal.TryParse(ChangePercent.TrimEnd('%'), out var val) ? val : 0;
}

public class GlobalQuoteResponse
{
    [JsonPropertyName("Global Quote")]
    public GlobalQuoteData? GlobalQuote { get; set; }
}

public class GlobalQuoteData
{
    [JsonPropertyName("01. symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("02. open")]
    public string Open { get; set; } = string.Empty;

    [JsonPropertyName("03. high")]
    public string High { get; set; } = string.Empty;

    [JsonPropertyName("04. low")]
    public string Low { get; set; } = string.Empty;

    [JsonPropertyName("05. price")]
    public string Price { get; set; } = string.Empty;

    [JsonPropertyName("06. volume")]
    public string Volume { get; set; } = string.Empty;

    [JsonPropertyName("07. latest trading day")]
    public string LatestTradingDay { get; set; } = string.Empty;

    [JsonPropertyName("08. previous close")]
    public string PreviousClose { get; set; } = string.Empty;

    [JsonPropertyName("09. change")]
    public string Change { get; set; } = string.Empty;

    [JsonPropertyName("10. change percent")]
    public string ChangePercent { get; set; } = string.Empty;

    public StockQuote ToStockQuote() => new()
    {
        Symbol = Symbol,
        Open = decimal.TryParse(Open, out var o) ? o : 0,
        High = decimal.TryParse(High, out var h) ? h : 0,
        Low = decimal.TryParse(Low, out var l) ? l : 0,
        Price = decimal.TryParse(Price, out var p) ? p : 0,
        Volume = long.TryParse(Volume, out var v) ? v : 0,
        LatestTradingDay = LatestTradingDay,
        PreviousClose = decimal.TryParse(PreviousClose, out var pc) ? pc : 0,
        Change = decimal.TryParse(Change, out var c) ? c : 0,
        ChangePercent = ChangePercent
    };
}
