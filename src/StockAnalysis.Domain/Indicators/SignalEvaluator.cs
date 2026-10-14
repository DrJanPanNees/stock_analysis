using StockAnalysis.Domain.ValueObjects;

namespace StockAnalysis.Domain.Indicators;

public static class SignalEvaluator
{
    public static List<TradeSignal> Evaluate(
        IReadOnlyList<PriceBar> bars,
        decimal?[] rsi,
        MacdResult macd,
        decimal rsiOversold,
        decimal rsiOverbought)
    {
        var signals = new List<TradeSignal>();

        for (var i = 1; i < bars.Count; i++)
        {
            var date = bars[i].Date;
            var prev = i - 1;

            // Krydser RSI ind i oversold/overbought?
            if (rsi[i] is { } rsiNow && rsi[prev] is { } rsiPrev)
            {
                if (rsiNow < rsiOversold && rsiPrev >= rsiOversold)
                    signals.Add(new TradeSignal(date, SignalKind.Buy, SignalSource.Rsi, bars[i].Close));
                else if (rsiNow > rsiOverbought && rsiPrev <= rsiOverbought)
                    signals.Add(new TradeSignal(date, SignalKind.Sell, SignalSource.Rsi, bars[i].Close));
            }

            // MACD krydser signallinjen?
            if (macd.Macd[i] is { } macdNow && macd.Signal[i] is { } signalNow &&
                macd.Macd[prev] is { } macdPrev && macd.Signal[prev] is { } signalPrev)
            {
                if (macdNow > signalNow && macdPrev <= signalPrev)
                    signals.Add(new TradeSignal(date, SignalKind.Buy, SignalSource.Macd, bars[i].Close));
                else if (macdNow < signalNow && macdPrev >= signalPrev)
                    signals.Add(new TradeSignal(date, SignalKind.Sell, SignalSource.Macd, bars[i].Close));
            }
        }
        return signals;
    }
}
