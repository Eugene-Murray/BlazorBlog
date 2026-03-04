namespace BlazorBlog.Models.AlphaVantage;

public class ScreenerResult
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
    public decimal MarketCap { get; set; }
    public decimal PE { get; set; }
    public decimal EPS { get; set; }
    public decimal DividendYield { get; set; }
    public decimal Beta { get; set; }
    public decimal FiftyTwoWeekHigh { get; set; }
    public decimal FiftyTwoWeekLow { get; set; }
    public decimal FiftyDayMA { get; set; }
    public decimal TwoHundredDayMA { get; set; }
    public decimal ProfitMargin { get; set; }
    public decimal ROE { get; set; }
    public decimal PriceToBook { get; set; }

    public string MarketCapFormatted => FormatMarketCap(MarketCap);
    public string VolumeFormatted => FormatVolume(Volume);
    public string ChangePercentFormatted => $"{(ChangePercent >= 0 ? "+" : "")}{ChangePercent:F2}%";

    private static string FormatMarketCap(decimal marketCap)
    {
        return marketCap switch
        {
            >= 1_000_000_000_000 => $"{marketCap / 1_000_000_000_000:F2}T",
            >= 1_000_000_000 => $"{marketCap / 1_000_000_000:F2}B",
            >= 1_000_000 => $"{marketCap / 1_000_000:F2}M",
            >= 1_000 => $"{marketCap / 1_000:F2}K",
            _ => marketCap.ToString("F2")
        };
    }

    private static string FormatVolume(long volume)
    {
        return volume switch
        {
            >= 1_000_000_000 => $"{volume / 1_000_000_000.0:F2}B",
            >= 1_000_000 => $"{volume / 1_000_000.0:F2}M",
            >= 1_000 => $"{volume / 1_000.0:F2}K",
            _ => volume.ToString()
        };
    }
}
