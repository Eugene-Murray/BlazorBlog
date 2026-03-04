using System.Text.Json.Serialization;

namespace BlazorBlog.Models.Massive;

/// <summary>
/// Ticker from the reference/tickers endpoint
/// </summary>
public class MassiveTicker
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("market")]
    public string Market { get; set; } = string.Empty;

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("primary_exchange")]
    public string? PrimaryExchange { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("currency_name")]
    public string? CurrencyName { get; set; }

    [JsonPropertyName("cik")]
    public string? Cik { get; set; }

    [JsonPropertyName("composite_figi")]
    public string? CompositeFigi { get; set; }

    [JsonPropertyName("share_class_figi")]
    public string? ShareClassFigi { get; set; }

    [JsonPropertyName("last_updated_utc")]
    public string? LastUpdatedUtc { get; set; }
}

/// <summary>
/// Response from reference/tickers endpoint
/// </summary>
public class MassiveTickersResponse
{
    [JsonPropertyName("results")]
    public List<MassiveTicker> Results { get; set; } = [];

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("next_url")]
    public string? NextUrl { get; set; }
}

/// <summary>
/// Detailed ticker info from reference/tickers/{ticker}
/// </summary>
public class MassiveTickerDetails
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("market")]
    public string Market { get; set; } = string.Empty;

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("primary_exchange")]
    public string? PrimaryExchange { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("currency_name")]
    public string? CurrencyName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("sic_code")]
    public string? SicCode { get; set; }

    [JsonPropertyName("sic_description")]
    public string? SicDescription { get; set; }

    [JsonPropertyName("ticker_root")]
    public string? TickerRoot { get; set; }

    [JsonPropertyName("homepage_url")]
    public string? HomepageUrl { get; set; }

    [JsonPropertyName("total_employees")]
    public int? TotalEmployees { get; set; }

    [JsonPropertyName("list_date")]
    public string? ListDate { get; set; }

    [JsonPropertyName("market_cap")]
    public long? MarketCap { get; set; }

    [JsonPropertyName("share_class_shares_outstanding")]
    public long? SharesOutstanding { get; set; }

    [JsonPropertyName("weighted_shares_outstanding")]
    public long? WeightedSharesOutstanding { get; set; }
}

/// <summary>
/// Response for ticker details
/// </summary>
public class MassiveTickerDetailsResponse
{
    [JsonPropertyName("results")]
    public MassiveTickerDetails? Results { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }
}

/// <summary>
/// Previous day's OHLC data
/// </summary>
public class MassivePreviousClose
{
    [JsonPropertyName("T")]
    public string Ticker { get; set; } = string.Empty;

    [JsonPropertyName("o")]
    public decimal Open { get; set; }

    [JsonPropertyName("h")]
    public decimal High { get; set; }

    [JsonPropertyName("l")]
    public decimal Low { get; set; }

    [JsonPropertyName("c")]
    public decimal Close { get; set; }

    [JsonPropertyName("v")]
    public long Volume { get; set; }

    [JsonPropertyName("vw")]
    public decimal VolumeWeightedAvgPrice { get; set; }

    [JsonPropertyName("n")]
    public int NumberOfTransactions { get; set; }

    [JsonPropertyName("t")]
    public long Timestamp { get; set; }
}

/// <summary>
/// Response for previous close
/// </summary>
public class MassivePreviousCloseResponse
{
    [JsonPropertyName("results")]
    public List<MassivePreviousClose>? Results { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("ticker")]
    public string? Ticker { get; set; }

    [JsonPropertyName("queryCount")]
    public int QueryCount { get; set; }

    [JsonPropertyName("resultsCount")]
    public int ResultsCount { get; set; }

    [JsonPropertyName("adjusted")]
    public bool Adjusted { get; set; }
}

/// <summary>
/// Quote response from /v1/quotes/{symbol}
/// </summary>
public class MassiveQuoteResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("last")]
    public decimal? Last { get; set; }

    [JsonPropertyName("bid")]
    public decimal? Bid { get; set; }

    [JsonPropertyName("ask")]
    public decimal? Ask { get; set; }

    [JsonPropertyName("bidSize")]
    public int? BidSize { get; set; }

    [JsonPropertyName("askSize")]
    public int? AskSize { get; set; }

    [JsonPropertyName("lastSize")]
    public int? LastSize { get; set; }

    [JsonPropertyName("lastExchange")]
    public int? LastExchange { get; set; }

    [JsonPropertyName("lastTimestamp")]
    public long? LastTimestamp { get; set; }
}

/// <summary>
/// Aggregate bar data (OHLCV)
/// </summary>
public class MassiveAggregateBar
{
    [JsonPropertyName("o")]
    public decimal Open { get; set; }

    [JsonPropertyName("h")]
    public decimal High { get; set; }

    [JsonPropertyName("l")]
    public decimal Low { get; set; }

    [JsonPropertyName("c")]
    public decimal Close { get; set; }

    [JsonPropertyName("v")]
    public long Volume { get; set; }

    [JsonPropertyName("vw")]
    public decimal? VolumeWeightedAvgPrice { get; set; }

    [JsonPropertyName("t")]
    public long Timestamp { get; set; }

    [JsonPropertyName("n")]
    public int? NumberOfTransactions { get; set; }
}

/// <summary>
/// Response for aggregates endpoint
/// </summary>
public class MassiveAggregatesResponse
{
    [JsonPropertyName("ticker")]
    public string? Ticker { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("queryCount")]
    public int QueryCount { get; set; }

    [JsonPropertyName("resultsCount")]
    public int ResultsCount { get; set; }

    [JsonPropertyName("adjusted")]
    public bool Adjusted { get; set; }

    [JsonPropertyName("results")]
    public List<MassiveAggregateBar>? Results { get; set; }
}

/// <summary>
/// Day data within a snapshot
/// </summary>
public class MassiveDayData
{
    [JsonPropertyName("o")]
    public decimal Open { get; set; }

    [JsonPropertyName("h")]
    public decimal High { get; set; }

    [JsonPropertyName("l")]
    public decimal Low { get; set; }

    [JsonPropertyName("c")]
    public decimal Close { get; set; }

    [JsonPropertyName("v")]
    public long Volume { get; set; }

    [JsonPropertyName("vw")]
    public decimal VolumeWeightedAvgPrice { get; set; }
}

/// <summary>
/// Minute data within a snapshot
/// </summary>
public class MassiveMinuteData
{
    [JsonPropertyName("o")]
    public decimal Open { get; set; }

    [JsonPropertyName("h")]
    public decimal High { get; set; }

    [JsonPropertyName("l")]
    public decimal Low { get; set; }

    [JsonPropertyName("c")]
    public decimal Close { get; set; }

    [JsonPropertyName("v")]
    public long Volume { get; set; }

    [JsonPropertyName("vw")]
    public decimal VolumeWeightedAvgPrice { get; set; }

    [JsonPropertyName("av")]
    public long AccumulatedVolume { get; set; }

    [JsonPropertyName("n")]
    public int NumberOfTransactions { get; set; }
}

/// <summary>
/// Ticker snapshot from snapshot endpoint
/// </summary>
public class MassiveTickerSnapshot
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; set; } = string.Empty;

    [JsonPropertyName("day")]
    public MassiveDayData? Day { get; set; }

    [JsonPropertyName("prevDay")]
    public MassiveDayData? PrevDay { get; set; }

    [JsonPropertyName("min")]
    public MassiveMinuteData? Min { get; set; }

    [JsonPropertyName("todaysChange")]
    public decimal? TodaysChange { get; set; }

    [JsonPropertyName("todaysChangePerc")]
    public decimal? TodaysChangePerc { get; set; }

    [JsonPropertyName("updated")]
    public long? Updated { get; set; }
}

/// <summary>
/// Response for single ticker snapshot
/// </summary>
public class MassiveTickerSnapshotResponse
{
    [JsonPropertyName("ticker")]
    public MassiveTickerSnapshot? Ticker { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }
}

/// <summary>
/// Response for all ticker snapshots
/// </summary>
public class MassiveSnapshotsResponse
{
    [JsonPropertyName("tickers")]
    public List<MassiveTickerSnapshot>? Tickers { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

/// <summary>
/// Stock quote (used for market movers display)
/// </summary>
public class MassiveQuote
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal PreviousClose { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
    public long Volume { get; set; }
}

/// <summary>
/// Stock search result
/// </summary>
public class MassiveSearchResult
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}

/// <summary>
/// Market movers (gainers/losers/most active)
/// </summary>
public class MassiveMarketMovers
{
    [JsonPropertyName("gainers")]
    public List<MassiveQuote> Gainers { get; set; } = [];

    [JsonPropertyName("losers")]
    public List<MassiveQuote> Losers { get; set; } = [];

    [JsonPropertyName("mostActive")]
    public List<MassiveQuote> MostActive { get; set; } = [];
}

/// <summary>
/// Screener result for display
/// </summary>
public class MassiveScreenerResult
{
    public string Symbol { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
    public long Volume { get; set; }
    public long MarketCap { get; set; }
    public decimal PE { get; set; }
    public decimal EPS { get; set; }
    public decimal DividendYield { get; set; }
    public decimal Beta { get; set; }
    public decimal High52Week { get; set; }
    public decimal Low52Week { get; set; }

    public string ChangePercentFormatted => ChangePercent >= 0
        ? $"+{ChangePercent:F2}%"
        : $"{ChangePercent:F2}%";

    public string VolumeFormatted => Volume switch
    {
        >= 1_000_000_000 => $"{Volume / 1_000_000_000.0:F2}B",
        >= 1_000_000 => $"{Volume / 1_000_000.0:F2}M",
        >= 1_000 => $"{Volume / 1_000.0:F2}K",
        _ => Volume.ToString("N0")
    };

    public string MarketCapFormatted => MarketCap switch
    {
        >= 1_000_000_000_000 => $"${MarketCap / 1_000_000_000_000.0:F2}T",
        >= 1_000_000_000 => $"${MarketCap / 1_000_000_000.0:F2}B",
        >= 1_000_000 => $"${MarketCap / 1_000_000.0:F2}M",
        _ => $"${MarketCap:N0}"
    };
}

/// <summary>
/// Screener filter options
/// </summary>
public class MassiveScreenerFilters
{
    public string? Exchange { get; set; }
    public string? Sector { get; set; }
    public string? Industry { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public long? MinVolume { get; set; }
    public long? MinMarketCap { get; set; }
    public long? MaxMarketCap { get; set; }
    public decimal? MinPE { get; set; }
    public decimal? MaxPE { get; set; }
    public decimal? MinDividendYield { get; set; }
    public decimal? MinChangePercent { get; set; }
    public decimal? MaxChangePercent { get; set; }
    public decimal? MinBeta { get; set; }
    public decimal? MaxBeta { get; set; }
    public MassiveMarketCapFilter MarketCapFilter { get; set; } = MassiveMarketCapFilter.Any;
    public MassiveChangeFilter ChangeFilter { get; set; } = MassiveChangeFilter.Any;
}

public enum MassiveMarketCapFilter
{
    Any,
    MegaCap,    // $200B+
    LargeCap,   // $10B - $200B
    MidCap,     // $2B - $10B
    SmallCap,   // $300M - $2B
    MicroCap,   // $50M - $300M
    NanoCap     // < $50M
}

public enum MassiveChangeFilter
{
    Any,
    Up,
    Down,
    UpMoreThan5Percent,
    DownMoreThan5Percent,
    UpMoreThan10Percent,
    DownMoreThan10Percent
}

/// <summary>
/// Static reference data
/// </summary>
public static class MassiveExchanges
{
    public static readonly string[] All =
    [
        "NYSE",
        "NASDAQ",
        "AMEX",
        "ARCA",
        "BATS"
    ];
}

public static class MassiveSectors
{
    public static readonly string[] All =
    [
        "Technology",
        "Healthcare",
        "Financial Services",
        "Consumer Cyclical",
        "Consumer Defensive",
        "Industrials",
        "Energy",
        "Basic Materials",
        "Communication Services",
        "Real Estate",
        "Utilities"
    ];
}
