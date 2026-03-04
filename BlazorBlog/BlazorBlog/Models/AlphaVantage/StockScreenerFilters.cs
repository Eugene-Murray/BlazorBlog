namespace BlazorBlog.Models.AlphaVantage;

public class StockScreenerFilters
{
    // Exchange filter
    public string? Exchange { get; set; }

    // Market Cap filters
    public MarketCapFilter? MarketCap { get; set; }

    // Price filters
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    // Volume filter
    public long? MinVolume { get; set; }

    // PE Ratio filters
    public decimal? MinPE { get; set; }
    public decimal? MaxPE { get; set; }

    // Dividend Yield filters
    public decimal? MinDividendYield { get; set; }
    public decimal? MaxDividendYield { get; set; }

    // Beta filters
    public decimal? MinBeta { get; set; }
    public decimal? MaxBeta { get; set; }

    // EPS filters
    public decimal? MinEPS { get; set; }

    // Profit Margin filter
    public decimal? MinProfitMargin { get; set; }

    // ROE filter
    public decimal? MinROE { get; set; }

    // 52 Week Performance
    public Week52Performance? Week52Performance { get; set; }

    // Price to Book
    public decimal? MaxPriceToBook { get; set; }

    // Sector filter
    public string? Sector { get; set; }

    // Industry filter
    public string? Industry { get; set; }

    // Change % filters
    public ChangeFilter? ChangeFilter { get; set; }
}

public enum MarketCapFilter
{
    Any,
    MegaCap,        // > $200B
    LargeCap,       // $10B - $200B
    MidCap,         // $2B - $10B
    SmallCap,       // $300M - $2B
    MicroCap,       // $50M - $300M
    NanoCap         // < $50M
}

public enum Week52Performance
{
    Any,
    Near52WeekHigh,     // Within 5% of high
    Near52WeekLow,      // Within 5% of low
    Above50DayMA,
    Below50DayMA,
    Above200DayMA,
    Below200DayMA
}

public enum ChangeFilter
{
    Any,
    Up,
    Down,
    UpMoreThan5Percent,
    DownMoreThan5Percent,
    UpMoreThan10Percent,
    DownMoreThan10Percent
}

public static class Sectors
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
        "Utilities",
        "Real Estate",
        "Basic Materials",
        "Communication Services"
    ];
}

public static class Exchanges
{
    public static readonly string[] All =
    [
        "NYSE",
        "NASDAQ",
        "AMEX",
        "NYSE ARCA",
        "BATS"
    ];
}
