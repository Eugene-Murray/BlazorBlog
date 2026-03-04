using System.Text.Json;
using BlazorBlog.Models.AlphaVantage;

namespace BlazorBlog.Services;

public class AlphaVantageService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<AlphaVantageService> _logger;
    private const string BaseUrl = "https://www.alphavantage.co/query";

    // Popular stock symbols for screening (since Alpha Vantage free tier has limits)
    private static readonly string[] PopularSymbols =
    [
        "AAPL", "MSFT", "GOOGL", "AMZN", "META", "TSLA", "NVDA", "JPM", "V", "JNJ",
        "WMT", "PG", "MA", "UNH", "HD", "DIS", "PYPL", "BAC", "ADBE", "NFLX",
        "CRM", "INTC", "VZ", "T", "PFE", "KO", "PEP", "MRK", "ABT", "TMO",
        "CSCO", "XOM", "CVX", "ABBV", "NKE", "MCD", "COST", "DHR", "AVGO", "LLY",
        "TXN", "NEE", "PM", "RTX", "HON", "UNP", "LOW", "QCOM", "BMY", "AMGN"
    ];

    public AlphaVantageService(HttpClient httpClient, IConfiguration configuration, ILogger<AlphaVantageService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["AlphaVantage:ApiKey"] ?? "Q4EYRAXW357WA6HV";
        _logger = logger;
        _logger.LogInformation("AlphaVantageService initialized with API key: {ApiKey}", _apiKey[..4] + "***");
    }

    public async Task<StockQuote?> GetQuoteAsync(string symbol)
    {
        try
        {
            var url = $"{BaseUrl}?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_apiKey}";
            _logger.LogInformation("Fetching quote for {Symbol} from {Url}", symbol, url.Replace(_apiKey, "***"));

            var response = await _httpClient.GetStringAsync(url);
            _logger.LogDebug("Response for {Symbol}: {Response}", symbol, response[..Math.Min(200, response.Length)]);

            var data = JsonSerializer.Deserialize<GlobalQuoteResponse>(response);
            return data?.GlobalQuote?.ToStockQuote();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching quote for {Symbol}", symbol);
            return null;
        }
    }

    public async Task<CompanyOverview?> GetCompanyOverviewAsync(string symbol)
    {
        try
        {
            var url = $"{BaseUrl}?function=OVERVIEW&symbol={symbol}&apikey={_apiKey}";
            _logger.LogInformation("Fetching company overview for {Symbol} from {Url}", symbol, url.Replace(_apiKey, "***"));

            var response = await _httpClient.GetStringAsync(url);
            _logger.LogDebug("Response for {Symbol}: {Response}", symbol, response[..Math.Min(200, response.Length)]);

            var data = JsonSerializer.Deserialize<CompanyOverview>(response);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching company overview for {Symbol}", symbol);
            return null;
        }
    }

    public async Task<TopGainersLosersResponse?> GetTopGainersLosersAsync()
    {
        try
        {
            var url = $"{BaseUrl}?function=TOP_GAINERS_LOSERS&apikey={_apiKey}";
            _logger.LogInformation("Fetching top gainers/losers from {Url}", url.Replace(_apiKey, "***"));

            var response = await _httpClient.GetStringAsync(url);
            _logger.LogDebug("Response: {Response}", response[..Math.Min(500, response.Length)]);

            var data = JsonSerializer.Deserialize<TopGainersLosersResponse>(response);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching top gainers/losers");
            return null;
        }
    }

    public async Task<List<SymbolMatch>> SearchSymbolsAsync(string keywords)
    {
        try
        {
            var url = $"{BaseUrl}?function=SYMBOL_SEARCH&keywords={Uri.EscapeDataString(keywords)}&apikey={_apiKey}";
            _logger.LogInformation("Searching symbols with keywords: {Keywords}", keywords);

            var response = await _httpClient.GetStringAsync(url);
            _logger.LogDebug("Search response: {Response}", response[..Math.Min(500, response.Length)]);

            var data = JsonSerializer.Deserialize<SymbolSearchResponse>(response);
            return data?.BestMatches ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for symbols with keywords {Keywords}", keywords);
            return [];
        }
    }

    public async Task<List<ScreenerResult>> ScreenStocksAsync(StockScreenerFilters filters, List<string>? customSymbols = null)
    {
        _logger.LogInformation("Starting stock screening...");

        var results = new List<ScreenerResult>();
        var symbolsToScreen = customSymbols ?? PopularSymbols.ToList();

        _logger.LogInformation("Screening {Count} symbols: {Symbols}", symbolsToScreen.Count, string.Join(", ", symbolsToScreen.Take(5)) + "...");

        // Note: Alpha Vantage free tier is limited to 25 requests/day
        // For a production app, you'd want to cache results and use a database
        var processedCount = 0;
        var maxRequests = 25; // Free tier limit

        foreach (var symbol in symbolsToScreen)
        {
            if (processedCount >= maxRequests)
            {
                _logger.LogWarning("Reached API rate limit. Processed {Count} stocks.", processedCount);
                break;
            }

            try
            {
                // Get company overview for fundamental data
                var overview = await GetCompanyOverviewAsync(symbol);
                if (overview == null || string.IsNullOrEmpty(overview.Symbol))
                {
                    await Task.Delay(100); // Small delay between requests
                    continue;
                }

                // Get current quote for price data
                var quote = await GetQuoteAsync(symbol);
                if (quote == null)
                {
                    await Task.Delay(100);
                    continue;
                }

                var result = new ScreenerResult
                {
                    Symbol = overview.Symbol,
                    CompanyName = overview.Name,
                    Sector = overview.Sector,
                    Industry = overview.Industry,
                    Exchange = overview.Exchange,
                    Price = quote.Price,
                    Change = quote.Change,
                    ChangePercent = quote.ChangePercentValue,
                    Volume = quote.Volume,
                    MarketCap = overview.MarketCapValue,
                    PE = overview.PERatioValue,
                    EPS = overview.EPSValue,
                    DividendYield = overview.DividendYieldValue * 100, // Convert to percentage
                    Beta = overview.BetaValue,
                    FiftyTwoWeekHigh = overview.FiftyTwoWeekHighValue,
                    FiftyTwoWeekLow = overview.FiftyTwoWeekLowValue,
                    FiftyDayMA = decimal.TryParse(overview.FiftyDayMovingAverage, out var ma50) ? ma50 : 0,
                    TwoHundredDayMA = decimal.TryParse(overview.TwoHundredDayMovingAverage, out var ma200) ? ma200 : 0,
                    ProfitMargin = overview.ProfitMarginValue * 100,
                    ROE = overview.ROEValue * 100,
                    PriceToBook = overview.PriceToBookValue
                };

                // Apply filters
                if (PassesFilters(result, filters))
                {
                    results.Add(result);
                }

                processedCount += 2; // We make 2 API calls per stock
                await Task.Delay(250); // Rate limiting delay
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing symbol {Symbol}", symbol);
            }
        }

        _logger.LogInformation("Screening complete. Found {Count} results.", results.Count);
        return results;
    }

    private static bool PassesFilters(ScreenerResult result, StockScreenerFilters filters)
    {
        // Exchange filter
        if (!string.IsNullOrEmpty(filters.Exchange) && 
            !result.Exchange.Contains(filters.Exchange, StringComparison.OrdinalIgnoreCase))
            return false;

        // Sector filter
        if (!string.IsNullOrEmpty(filters.Sector) && 
            !result.Sector.Equals(filters.Sector, StringComparison.OrdinalIgnoreCase))
            return false;

        // Industry filter
        if (!string.IsNullOrEmpty(filters.Industry) && 
            !result.Industry.Contains(filters.Industry, StringComparison.OrdinalIgnoreCase))
            return false;

        // Price filters
        if (filters.MinPrice.HasValue && result.Price < filters.MinPrice.Value)
            return false;
        if (filters.MaxPrice.HasValue && result.Price > filters.MaxPrice.Value)
            return false;

        // Volume filter
        if (filters.MinVolume.HasValue && result.Volume < filters.MinVolume.Value)
            return false;

        // Market Cap filter
        if (filters.MarketCap.HasValue && filters.MarketCap != MarketCapFilter.Any)
        {
            var passes = filters.MarketCap switch
            {
                MarketCapFilter.MegaCap => result.MarketCap >= 200_000_000_000,
                MarketCapFilter.LargeCap => result.MarketCap >= 10_000_000_000 && result.MarketCap < 200_000_000_000,
                MarketCapFilter.MidCap => result.MarketCap >= 2_000_000_000 && result.MarketCap < 10_000_000_000,
                MarketCapFilter.SmallCap => result.MarketCap >= 300_000_000 && result.MarketCap < 2_000_000_000,
                MarketCapFilter.MicroCap => result.MarketCap >= 50_000_000 && result.MarketCap < 300_000_000,
                MarketCapFilter.NanoCap => result.MarketCap < 50_000_000,
                _ => true
            };
            if (!passes) return false;
        }

        // PE Ratio filters
        if (filters.MinPE.HasValue && result.PE < filters.MinPE.Value)
            return false;
        if (filters.MaxPE.HasValue && result.PE > filters.MaxPE.Value)
            return false;

        // Dividend Yield filters
        if (filters.MinDividendYield.HasValue && result.DividendYield < filters.MinDividendYield.Value)
            return false;
        if (filters.MaxDividendYield.HasValue && result.DividendYield > filters.MaxDividendYield.Value)
            return false;

        // Beta filters
        if (filters.MinBeta.HasValue && result.Beta < filters.MinBeta.Value)
            return false;
        if (filters.MaxBeta.HasValue && result.Beta > filters.MaxBeta.Value)
            return false;

        // EPS filter
        if (filters.MinEPS.HasValue && result.EPS < filters.MinEPS.Value)
            return false;

        // Profit Margin filter
        if (filters.MinProfitMargin.HasValue && result.ProfitMargin < filters.MinProfitMargin.Value)
            return false;

        // ROE filter
        if (filters.MinROE.HasValue && result.ROE < filters.MinROE.Value)
            return false;

        // Price to Book filter
        if (filters.MaxPriceToBook.HasValue && result.PriceToBook > filters.MaxPriceToBook.Value)
            return false;

        // Change filter
        if (filters.ChangeFilter.HasValue && filters.ChangeFilter != ChangeFilter.Any)
        {
            var passes = filters.ChangeFilter switch
            {
                ChangeFilter.Up => result.ChangePercent > 0,
                ChangeFilter.Down => result.ChangePercent < 0,
                ChangeFilter.UpMoreThan5Percent => result.ChangePercent >= 5,
                ChangeFilter.DownMoreThan5Percent => result.ChangePercent <= -5,
                ChangeFilter.UpMoreThan10Percent => result.ChangePercent >= 10,
                ChangeFilter.DownMoreThan10Percent => result.ChangePercent <= -10,
                _ => true
            };
            if (!passes) return false;
        }

        // 52 Week Performance filter
        if (filters.Week52Performance.HasValue && filters.Week52Performance != Week52Performance.Any && result.FiftyTwoWeekHigh > 0)
        {
            var passes = filters.Week52Performance switch
            {
                Week52Performance.Near52WeekHigh => result.Price >= result.FiftyTwoWeekHigh * 0.95m,
                Week52Performance.Near52WeekLow => result.Price <= result.FiftyTwoWeekLow * 1.05m,
                Week52Performance.Above50DayMA => result.FiftyDayMA > 0 && result.Price > result.FiftyDayMA,
                Week52Performance.Below50DayMA => result.FiftyDayMA > 0 && result.Price < result.FiftyDayMA,
                Week52Performance.Above200DayMA => result.TwoHundredDayMA > 0 && result.Price > result.TwoHundredDayMA,
                Week52Performance.Below200DayMA => result.TwoHundredDayMA > 0 && result.Price < result.TwoHundredDayMA,
                _ => true
            };
            if (!passes) return false;
        }

        return true;
    }

    public string[] GetPopularSymbols() => PopularSymbols;
}
