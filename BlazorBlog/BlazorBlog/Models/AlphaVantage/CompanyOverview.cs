using System.Text.Json.Serialization;

namespace BlazorBlog.Models.AlphaVantage;

public class CompanyOverview
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string MarketCapitalization { get; set; } = string.Empty;
    public string EBITDA { get; set; } = string.Empty;
    public string PERatio { get; set; } = string.Empty;
    public string PEGRatio { get; set; } = string.Empty;
    public string BookValue { get; set; } = string.Empty;
    public string DividendPerShare { get; set; } = string.Empty;
    public string DividendYield { get; set; } = string.Empty;
    public string EPS { get; set; } = string.Empty;
    public string RevenuePerShareTTM { get; set; } = string.Empty;
    public string ProfitMargin { get; set; } = string.Empty;
    public string OperatingMarginTTM { get; set; } = string.Empty;
    public string ReturnOnAssetsTTM { get; set; } = string.Empty;
    public string ReturnOnEquityTTM { get; set; } = string.Empty;
    public string RevenueTTM { get; set; } = string.Empty;
    public string GrossProfitTTM { get; set; } = string.Empty;
    public string DilutedEPSTTM { get; set; } = string.Empty;
    public string QuarterlyEarningsGrowthYOY { get; set; } = string.Empty;
    public string QuarterlyRevenueGrowthYOY { get; set; } = string.Empty;
    public string AnalystTargetPrice { get; set; } = string.Empty;
    public string AnalystRatingStrongBuy { get; set; } = string.Empty;
    public string AnalystRatingBuy { get; set; } = string.Empty;
    public string AnalystRatingHold { get; set; } = string.Empty;
    public string AnalystRatingSell { get; set; } = string.Empty;
    public string AnalystRatingStrongSell { get; set; } = string.Empty;
    public string TrailingPE { get; set; } = string.Empty;
    public string ForwardPE { get; set; } = string.Empty;
    public string PriceToSalesRatioTTM { get; set; } = string.Empty;
    public string PriceToBookRatio { get; set; } = string.Empty;
    public string EVToRevenue { get; set; } = string.Empty;
    public string EVToEBITDA { get; set; } = string.Empty;
    public string Beta { get; set; } = string.Empty;
    [JsonPropertyName("52WeekHigh")]
    public string FiftyTwoWeekHigh { get; set; } = string.Empty;
    [JsonPropertyName("52WeekLow")]
    public string FiftyTwoWeekLow { get; set; } = string.Empty;
    [JsonPropertyName("50DayMovingAverage")]
    public string FiftyDayMovingAverage { get; set; } = string.Empty;
    [JsonPropertyName("200DayMovingAverage")]
    public string TwoHundredDayMovingAverage { get; set; } = string.Empty;
    public string SharesOutstanding { get; set; } = string.Empty;
    public string SharesFloat { get; set; } = string.Empty;
    public string SharesShort { get; set; } = string.Empty;
    public string SharesShortPriorMonth { get; set; } = string.Empty;
    public string ShortRatio { get; set; } = string.Empty;
    public string ShortPercentOutstanding { get; set; } = string.Empty;
    public string ShortPercentFloat { get; set; } = string.Empty;
    public string PercentInsiders { get; set; } = string.Empty;
    public string PercentInstitutions { get; set; } = string.Empty;
    public string ForwardAnnualDividendRate { get; set; } = string.Empty;
    public string ForwardAnnualDividendYield { get; set; } = string.Empty;
    public string PayoutRatio { get; set; } = string.Empty;
    public string ExDividendDate { get; set; } = string.Empty;
    public string LastSplitFactor { get; set; } = string.Empty;
    public string LastSplitDate { get; set; } = string.Empty;

    public decimal MarketCapValue => decimal.TryParse(MarketCapitalization, out var val) ? val : 0;
    public decimal PERatioValue => decimal.TryParse(PERatio, out var val) ? val : 0;
    public decimal EPSValue => decimal.TryParse(EPS, out var val) ? val : 0;
    public decimal DividendYieldValue => decimal.TryParse(DividendYield, out var val) ? val : 0;
    public decimal BetaValue => decimal.TryParse(Beta, out var val) ? val : 0;
    public decimal ProfitMarginValue => decimal.TryParse(ProfitMargin, out var val) ? val : 0;
    public decimal ROEValue => decimal.TryParse(ReturnOnEquityTTM, out var val) ? val : 0;
    public decimal PriceToBookValue => decimal.TryParse(PriceToBookRatio, out var val) ? val : 0;
    public decimal FiftyTwoWeekHighValue => decimal.TryParse(FiftyTwoWeekHigh, out var val) ? val : 0;
    public decimal FiftyTwoWeekLowValue => decimal.TryParse(FiftyTwoWeekLow, out var val) ? val : 0;
}
