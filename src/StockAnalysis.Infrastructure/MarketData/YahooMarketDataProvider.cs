using System.Net;
using System.Text.Json;
using StockAnalysis.Application.Abstractions;
using StockAnalysis.Domain.ValueObjects;

namespace StockAnalysis.Infrastructure.MarketData;

public sealed class YahooMarketDataProvider : IMarketDataProvider
{
    // Yahoos uofficielle endpoints blokerer ofte klienter uden browser-fingeraftryk/User-Agent.
    // Vi bruger chart-endepunktet (v8/finance/chart), som ikke kræver cookie/crumb og returnerer JSON.
    private const string UserAgent =
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0 Safari/537.36";

    private const int MaxRetries = 3;

    private static readonly HttpClient Client = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        return client;
    }

    public async Task<IReadOnlyList<PriceBar>> GetDailyHistoryAsync(string symbol, int months, CancellationToken cancellationToken = default)
    {
        var end = DateTimeOffset.UtcNow;
        var start = end.AddMonths(-months);

        var url = $"https://query2.finance.yahoo.com/v8/finance/chart/{Uri.EscapeDataString(symbol)}" +
                  $"?interval=1d&period1={start.ToUnixTimeSeconds()}&period2={end.ToUnixTimeSeconds()}";

        var json = await GetWithRetryAsync(url, cancellationToken);
        return ParseChart(json);
    }

    private static async Task<string> GetWithRetryAsync(string url, CancellationToken cancellationToken)
    {
        for (var attempt = 0; ; attempt++)
        {
            using var response = await Client.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync(cancellationToken);

            var retryable = response.StatusCode == HttpStatusCode.TooManyRequests || (int)response.StatusCode >= 500;
            if (!retryable || attempt >= MaxRetries)
                throw new HttpRequestException($"Yahoo Finance returnerede {(int)response.StatusCode} ({response.StatusCode}).");

            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt + 1)), cancellationToken);
        }
    }

    private static IReadOnlyList<PriceBar> ParseChart(string json)
    {
        using var doc = JsonDocument.Parse(json);

        var result = doc.RootElement.GetProperty("chart").GetProperty("result");
        if (result.GetArrayLength() == 0) return Array.Empty<PriceBar>();

        var series = result[0];
        if (!series.TryGetProperty("timestamp", out var timestamps)) return Array.Empty<PriceBar>();
        if (!series.TryGetProperty("indicators", out var indicators)) return Array.Empty<PriceBar>();
        if (!indicators.TryGetProperty("quote", out var quotes) || quotes.GetArrayLength() == 0) return Array.Empty<PriceBar>();

        var quote = quotes[0];

        var bars = new List<PriceBar>(timestamps.GetArrayLength());
        for (var i = 0; i < timestamps.GetArrayLength(); i++)
        {
            if (!TryGetDecimal(quote, "close", i, out var close)) continue;

            var date = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(timestamps[i].GetInt64()).UtcDateTime);
            bars.Add(new PriceBar(
                date,
                GetDecimalOrDefault(quote, "open", i, close),
                GetDecimalOrDefault(quote, "high", i, close),
                GetDecimalOrDefault(quote, "low", i, close),
                close,
                GetInt64OrDefault(quote, "volume", i, 0)));
        }

        return bars;
    }

    private static bool TryGetDecimal(JsonElement obj, string name, int index, out decimal value)
    {
        value = 0m;
        if (!obj.TryGetProperty(name, out var arr)) return false;
        if (index >= arr.GetArrayLength()) return false;
        var el = arr[index];
        if (el.ValueKind != JsonValueKind.Number) return false;
        value = el.GetDecimal();
        return true;
    }

    private static decimal GetDecimalOrDefault(JsonElement obj, string name, int index, decimal fallback)
        => TryGetDecimal(obj, name, index, out var value) ? value : fallback;

    private static long GetInt64OrDefault(JsonElement obj, string name, int index, long fallback)
    {
        if (!obj.TryGetProperty(name, out var arr)) return fallback;
        if (index >= arr.GetArrayLength()) return fallback;
        var el = arr[index];
        if (el.ValueKind != JsonValueKind.Number) return fallback;
        return (long)el.GetDouble();
    }
}
