using StockAnalysis.Domain.Indicators;
using StockAnalysis.Domain.ValueObjects;

namespace StockAnalysis.Domain.Services;

public static class SummaryBuilder
{
    public static AnalysisSummary Build(
        string tickerName,
        IReadOnlyList<PriceBar> bars,
        decimal?[] rsi,
        MacdResult macd,
        BollingerBandsResult bollinger,
        IReadOnlyList<TradeSignal> signals,
        AnalysisParameters p)
    {
        var last = bars.Count - 1;

        var rsiLevel = rsi[last] switch
        {
            null => RsiLevel.Neutral,
            var v when v < p.RsiOversold => RsiLevel.Oversold,
            var v when v > p.RsiOverbought => RsiLevel.Overbought,
            _ => RsiLevel.Neutral,
        };

        var rsiDirection = RsiDirection.Flat;
        if (rsi[last] is { } rsiNow && last >= 1 && rsi[last - 1] is { } rsiPrev)
        {
            if (rsiNow > rsiPrev) rsiDirection = RsiDirection.Increasing;
            else if (rsiNow < rsiPrev) rsiDirection = RsiDirection.Decreasing;
        }

        var macdTrend = MacdTrend.Neutral;
        if (macd.Macd[last] is { } macdNow && macd.Signal[last] is { } signalNow)
        {
            if (macdNow > signalNow) macdTrend = MacdTrend.Bullish;
            else if (macdNow < signalNow) macdTrend = MacdTrend.Bearish;
        }

        var close = bars[last].Close;
        var position = PricePosition.NearMiddleBand;
        if (bollinger.Upper[last] is { } upper && close > upper) position = PricePosition.NearUpperBand;
        else if (bollinger.Lower[last] is { } lower && close < lower) position = PricePosition.NearLowerBand;

        var lastBuy = signals.Where(s => s.Kind == SignalKind.Buy).Select(s => (DateOnly?)s.Date).Max();
        var lastSell = signals.Where(s => s.Kind == SignalKind.Sell).Select(s => (DateOnly?)s.Date).Max();

        return new AnalysisSummary(tickerName, rsiLevel, rsiDirection, macdTrend, position, lastBuy, lastSell);
    }
}
