namespace StockAnalysis.Domain.ValueObjects;

public enum RsiLevel { Neutral, Oversold, Overbought }
public enum RsiDirection { Flat, Increasing, Decreasing }
public enum MacdTrend { Neutral, Bullish, Bearish }
public enum PricePosition { NearLowerBand, NearMiddleBand, NearUpperBand }

public sealed record AnalysisSummary(
    string TickerName,
    RsiLevel RsiLevel,
    RsiDirection RsiDirection,
    MacdTrend MacdTrend,
    PricePosition PricePosition,
    DateOnly? LastBuySignal,
    DateOnly? LastSellSignal)
{
    public string ToText()
    {
        var positionText = PricePosition switch
        {
            PricePosition.NearUpperBand => "Near Upper Band",
            PricePosition.NearLowerBand => "Near Lower Band",
            _ => "Near Middle Band"
        };

        return $"Detailed Summary for {TickerName}:\n" +
               $"  - RSI Level: {RsiLevel} ({RsiDirection})\n" +
               $"  - MACD Trend: {MacdTrend} (MACD is {(MacdTrend == MacdTrend.Bullish ? "above" : "below")} the Signal Line)\n" +
               $"  - Price Position: {positionText}\n" +
               $"  - Last Buy Signal: {LastBuySignal?.ToString("yyyy-MM-dd") ?? "No recent buy signal"}\n" +
               $"  - Last Sell Signal: {LastSellSignal?.ToString("yyyy-MM-dd") ?? "No recent sell signal"}\n";
    }
}
