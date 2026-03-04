using System.Net.Http.Headers;
using System.Text.Json;
using BlazorBlog.Models.Massive;

namespace BlazorBlog.Services;

/// <summary>
/// Service for interacting with the Massive stock API
/// API Documentation: https://massive.com/docs/rest/stocks
/// Working endpoints:
/// - GET /v3/reference/tickers - list tickers
/// - GET /v1/quotes/{symbol} - get quote
/// - GET /v1/aggs/ticker/{symbol}/prev - previous day OHLC
/// - GET /v1/aggs/ticker/{symbol}/range/{multiplier}/{timespan}/{from}/{to} - aggregates
/// </summary>
public class MassiveStockService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<MassiveStockService> _logger;
    private const string BaseUrlV3 = "https://api.massive.com/v3";
    private const string BaseUrlV1 = "https://api.massive.com/v1";

    // Popular stock symbols for screening
    private static readonly string[] PopularSymbols =
    [
        "AAPL", "MSFT", "GOOGL", "AMZN", "META", "TSLA", "NVDA", "JPM", "V", "JNJ",
        "WMT", "PG", "MA", "UNH", "HD", "DIS", "PYPL", "BAC", "ADBE", "NFLX",
        "CRM", "INTC", "VZ", "T", "PFE", "KO", "PEP", "MRK", "ABT", "TMO",
        "CSCO", "XOM", "CVX", "ABBV", "NKE", "MCD", "COST", "DHR", "AVGO", "LLY",
        "TXN", "NEE", "PM", "RTX", "HON", "UNP", "LOW", "QCOM", "BMY", "AMGN"
    ];

    public MassiveStockService(HttpClient httpClient, IConfiguration configuration, ILogger<MassiveStockService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Massive:ApiKey"] ?? "GgptF4BomXj7hFm25W6Y1znfpQWfy1Ug";
        _logger = logger;

        // Set default headers
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _logger.LogInformation("MassiveStockService initialized");
    }

    /// <summary>
    /// Appends the API key to the URL as a query parameter
    /// </summary>
    private string AppendApiKey(string url)
    {
        var separator = url.Contains('?') ? "&" : "?";
        return $"{url}{separator}apiKey={_apiKey}";
    }

    /// <summary>
    /// Get all stock tickers from the reference endpoint (WORKING)
    /// </summary>
    public async Task<List<MassiveTicker>> GetAllTickersAsync(int limit = 100)
    {
        try
        {
            var url = AppendApiKey($"{BaseUrlV3}/reference/tickers?market=stocks&active=true&order=asc&limit={limit}&sort=ticker");
            _logger.LogInformation("Fetching all tickers with limit {Limit}", limit);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get tickers: {StatusCode}", response.StatusCode);
                return [];
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Tickers response: {Response}", content[..Math.Min(500, content.Length)]);

            var result = JsonSerializer.Deserialize<MassiveTickersResponse>(content);
            return result?.Results ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tickers");
            return [];
        }
    }

    /// <summary>
    /// Get ticker details for a specific symbol
    /// </summary>
    public async Task<MassiveTickerDetails?> GetTickerDetailsAsync(string symbol)
    {
        try
        {
            var url = AppendApiKey($"{BaseUrlV3}/reference/tickers/{symbol}");
            _logger.LogInformation("Fetching ticker details for {Symbol}", symbol);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get ticker details for {Symbol}: {StatusCode}", symbol, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MassiveTickerDetailsResponse>(content);
            return result?.Results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching ticker details for {Symbol}", symbol);
            return null;
        }
    }

    /// <summary>
    /// Get quote for a specific symbol using /v1/quotes/{symbol}
    /// </summary>
    public async Task<MassiveQuoteResponse?> GetQuoteAsync(string symbol)
    {
        try
        {
            var url = AppendApiKey($"{BaseUrlV1}/quotes/{symbol}");
            _logger.LogInformation("Fetching quote for {Symbol}", symbol);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get quote for {Symbol}: {StatusCode}", symbol, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Quote response for {Symbol}: {Response}", symbol, content[..Math.Min(300, content.Length)]);

            var result = JsonSerializer.Deserialize<MassiveQuoteResponse>(content);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching quote for {Symbol}", symbol);
            return null;
        }
    }

    /// <summary>
    /// Get previous day's OHLC for a ticker using daily aggregates
    /// Uses format: /v1/aggs/{symbol}/day/1?from={date}&to={date}
    /// </summary>
    public async Task<MassivePreviousClose?> GetPreviousCloseAsync(string symbol)
    {
        try
        {
            // Get yesterday's date (or last trading day)
            var yesterday = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
            var twoDaysAgo = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd"); // Go back more days to catch weekends

            var url = AppendApiKey($"{BaseUrlV1}/aggs/{symbol}/day/1?from={twoDaysAgo}&to={yesterday}");
            _logger.LogInformation("Fetching aggregates for {Symbol} from {From} to {To}", symbol, twoDaysAgo, yesterday);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get aggregates for {Symbol}: {StatusCode}", symbol, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Aggregates response for {Symbol}: {Response}", symbol, content[..Math.Min(500, content.Length)]);

            var result = JsonSerializer.Deserialize<MassiveAggregatesResponse>(content);
            var lastBar = result?.Results?.LastOrDefault();

            if (lastBar == null) return null;

            // Map aggregate bar to previous close format
            return new MassivePreviousClose
            {
                Ticker = symbol,
                Open = lastBar.Open,
                High = lastBar.High,
                Low = lastBar.Low,
                Close = lastBar.Close,
                Volume = lastBar.Volume,
                VolumeWeightedAvgPrice = lastBar.VolumeWeightedAvgPrice ?? 0,
                Timestamp = lastBar.Timestamp
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching aggregates for {Symbol}", symbol);
            return null;
        }
    }

    /// <summary>
    /// Get daily aggregates for computing moving averages
    /// Uses format: /v1/aggs/{symbol}/day/1?from={date}&to={date}
    /// </summary>
    public async Task<List<MassiveAggregateBar>> GetDailyAggregatesAsync(string symbol, int days = 50)
    {
        try
        {
            var toDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var fromDate = DateTime.UtcNow.AddDays(-days - 10).ToString("yyyy-MM-dd"); // Extra days for weekends/holidays

            var url = AppendApiKey($"{BaseUrlV1}/aggs/{symbol}/day/1?from={fromDate}&to={toDate}");
            _logger.LogInformation("Fetching daily aggregates for {Symbol} from {From} to {To}", symbol, fromDate, toDate);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get aggregates for {Symbol}: {StatusCode}", symbol, response.StatusCode);
                return [];
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MassiveAggregatesResponse>(content);
            return result?.Results ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching aggregates for {Symbol}", symbol);
            return [];
        }
    }

    /// <summary>
    /// Search for tickers by name or symbol using reference/tickers with search param
    /// </summary>
    public async Task<List<MassiveSearchResult>> SearchAsync(string query)
    {
        try
        {
            var url = AppendApiKey($"{BaseUrlV3}/reference/tickers?search={Uri.EscapeDataString(query)}&market=stocks&active=true&limit=20");
            _logger.LogInformation("Searching for {Query}", query);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Search failed: {StatusCode}", response.StatusCode);
                return [];
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MassiveTickersResponse>(content);

            return result?.Results?.Select(t => new MassiveSearchResult
            {
                Symbol = t.Ticker,
                Name = t.Name,
                Exchange = t.PrimaryExchange ?? "",
                Type = t.Type ?? "Stock",
                Region = t.Locale ?? "US"
            }).ToList() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for {Query}", query);
            return [];
        }
    }

    /// <summary>
    /// Get market movers by fetching quotes for popular symbols and sorting
    /// </summary>
    public async Task<MassiveMarketMovers?> GetMarketMoversAsync()
    {
        try
        {
            _logger.LogInformation("Fetching market movers...");

            var movers = new MassiveMarketMovers();
            var quotes = new List<MassiveQuote>();

            // Fetch quotes for popular symbols
            foreach (var symbol in PopularSymbols.Take(30))
            {
                var quoteData = await GetStockDataAsync(symbol);
                if (quoteData != null)
                {
                    quotes.Add(quoteData);
                }
                await Task.Delay(50); // Rate limiting
            }

            // Sort for gainers (top positive change)
            movers.Gainers = quotes
                .Where(q => q.ChangePercent > 0)
                .OrderByDescending(q => q.ChangePercent)
                .Take(10)
                .ToList();

            // Sort for losers (top negative change)
            movers.Losers = quotes
                .Where(q => q.ChangePercent < 0)
                .OrderBy(q => q.ChangePercent)
                .Take(10)
                .ToList();

            // Sort for most active (highest volume)
            movers.MostActive = quotes
                .OrderByDescending(q => q.Volume)
                .Take(10)
                .ToList();

            return movers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching market movers");
            return null;
        }
    }

    /// <summary>
    /// Get combined stock data (quote + previous close) for a symbol
    /// </summary>
    private async Task<MassiveQuote?> GetStockDataAsync(string symbol)
    {
        try
        {
            var prevClose = await GetPreviousCloseAsync(symbol);
            if (prevClose == null) return null;

            var quote = new MassiveQuote
            {
                Symbol = symbol,
                Name = symbol,
                Price = prevClose.Close,
                Open = prevClose.Open,
                High = prevClose.High,
                Low = prevClose.Low,
                Close = prevClose.Close,
                Volume = prevClose.Volume,
                PreviousClose = prevClose.Close, // Will be updated if we have current day data
                Change = 0,
                ChangePercent = 0
            };

            // Try to get current quote for real-time data
            var currentQuote = await GetQuoteAsync(symbol);
            if (currentQuote != null)
            {
                quote.Price = currentQuote.Last ?? prevClose.Close;
                quote.PreviousClose = prevClose.Close;
                quote.Change = quote.Price - quote.PreviousClose;
                quote.ChangePercent = quote.PreviousClose > 0
                    ? (quote.Change / quote.PreviousClose) * 100
                    : 0;
            }

            return quote;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock data for {Symbol}", symbol);
            return null;
        }
    }

    /// <summary>
    /// Screen stocks based on filters
    /// </summary>
    public async Task<List<MassiveScreenerResult>> ScreenStocksAsync(MassiveScreenerFilters filters, List<string>? customSymbols = null)
    {
        _logger.LogInformation("Starting stock screening with Massive API...");

        var results = new List<MassiveScreenerResult>();

        try
        {
            // Get tickers from API - this endpoint works!
            var tickers = await GetAllTickersAsync(100);
            _logger.LogInformation("Got {Count} tickers from API", tickers.Count);

            if (tickers.Count == 0)
            {
                _logger.LogWarning("No tickers returned from API");
                return results;
            }

            // Filter by custom symbols if provided
            if (customSymbols != null && customSymbols.Count > 0)
            {
                tickers = tickers.Where(t => customSymbols.Contains(t.Ticker, StringComparer.OrdinalIgnoreCase)).ToList();
                _logger.LogInformation("Filtered to {Count} custom symbols", tickers.Count);
            }

            foreach (var ticker in tickers.Take(100))
            {
                try
                {
                    // Create result from ticker data (this data is available)
                    var result = new MassiveScreenerResult
                    {
                        Symbol = ticker.Ticker,
                        CompanyName = ticker.Name,
                        Exchange = ticker.PrimaryExchange ?? "",
                        Sector = ticker.Type ?? "",
                        Industry = ticker.Market,
                        Price = 0, // Will try to get from aggregates
                        Change = 0,
                        ChangePercent = 0,
                        Volume = 0,
                        MarketCap = 0,
                    };

                    // Try to get price data from previous close
                    var prevClose = await GetPreviousCloseAsync(ticker.Ticker);
                    if (prevClose != null)
                    {
                        result.Price = prevClose.Close;
                        result.Volume = prevClose.Volume;
                        result.High52Week = prevClose.High;
                        result.Low52Week = prevClose.Low;
                        _logger.LogDebug("Got prev close for {Symbol}: Price={Price}, Volume={Volume}", 
                            ticker.Ticker, prevClose.Close, prevClose.Volume);
                    }
                    else
                    {
                        _logger.LogDebug("No prev close data for {Symbol}, skipping price data", ticker.Ticker);
                    }

                    // Only add results with price data or if we want all tickers
                    if (result.Price > 0 || filters.MinPrice == null)
                    {
                        if (PassesFilters(result, filters))
                        {
                            results.Add(result);
                            _logger.LogDebug("Added {Symbol} to results", ticker.Ticker);
                        }
                    }

                    await Task.Delay(50); // Rate limiting
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error processing ticker {Symbol}", ticker.Ticker);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during stock screening");
        }

        _logger.LogInformation("Screening complete. Found {Count} matching stocks", results.Count);
        return results;
    }

    private static bool PassesFilters(MassiveScreenerResult result, MassiveScreenerFilters filters)
    {
        // Exchange filter
        if (!string.IsNullOrEmpty(filters.Exchange) &&
            !result.Exchange.Contains(filters.Exchange, StringComparison.OrdinalIgnoreCase))
            return false;

        // Sector filter
        if (!string.IsNullOrEmpty(filters.Sector) &&
            !result.Sector.Contains(filters.Sector, StringComparison.OrdinalIgnoreCase))
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
        if (filters.MarketCapFilter != MassiveMarketCapFilter.Any)
        {
            var passes = filters.MarketCapFilter switch
            {
                MassiveMarketCapFilter.MegaCap => result.MarketCap >= 200_000_000_000,
                MassiveMarketCapFilter.LargeCap => result.MarketCap >= 10_000_000_000 && result.MarketCap < 200_000_000_000,
                MassiveMarketCapFilter.MidCap => result.MarketCap >= 2_000_000_000 && result.MarketCap < 10_000_000_000,
                MassiveMarketCapFilter.SmallCap => result.MarketCap >= 300_000_000 && result.MarketCap < 2_000_000_000,
                MassiveMarketCapFilter.MicroCap => result.MarketCap >= 50_000_000 && result.MarketCap < 300_000_000,
                MassiveMarketCapFilter.NanoCap => result.MarketCap < 50_000_000,
                _ => true
            };
            if (!passes) return false;
        }

        // Change filter
        if (filters.ChangeFilter != MassiveChangeFilter.Any)
        {
            var passes = filters.ChangeFilter switch
            {
                MassiveChangeFilter.Up => result.ChangePercent > 0,
                MassiveChangeFilter.Down => result.ChangePercent < 0,
                MassiveChangeFilter.UpMoreThan5Percent => result.ChangePercent > 5,
                MassiveChangeFilter.DownMoreThan5Percent => result.ChangePercent < -5,
                MassiveChangeFilter.UpMoreThan10Percent => result.ChangePercent > 10,
                MassiveChangeFilter.DownMoreThan10Percent => result.ChangePercent < -10,
                _ => true
            };
            if (!passes) return false;
        }

        // PE Ratio filters
        if (filters.MinPE.HasValue && result.PE < filters.MinPE.Value)
            return false;
        if (filters.MaxPE.HasValue && result.PE > filters.MaxPE.Value)
            return false;

        // Dividend Yield filter
        if (filters.MinDividendYield.HasValue && result.DividendYield < filters.MinDividendYield.Value)
            return false;

        // Beta filters
        if (filters.MinBeta.HasValue && result.Beta < filters.MinBeta.Value)
            return false;
        if (filters.MaxBeta.HasValue && result.Beta > filters.MaxBeta.Value)
            return false;

        return true;
    }
}
