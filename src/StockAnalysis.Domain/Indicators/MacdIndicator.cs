using StockAnalysis.Domain.ValueObjects;

namespace StockAnalysis.Domain.Indicators;

public static class MacdIndicator
{
    public static MacdResult Calculate(IReadOnlyList<decimal> closes, int fastLength, int slowLength, int signalLength)
    {
        var emaFast = Ema(closes, fastLength);
        var emaSlow = Ema(closes, slowLength);

        var macdRaw = new decimal[closes.Count];
        var macd = new decimal?[closes.Count];
        for (var i = 0; i < closes.Count; i++)
        {
            macdRaw[i] = emaFast[i] - emaSlow[i];
            macd[i] = macdRaw[i];
        }

        var signalRaw = Ema(macdRaw, signalLength);
        var signal = new decimal?[signalRaw.Length];
        for (var i = 0; i < signalRaw.Length; i++) signal[i] = signalRaw[i];

        return new MacdResult(macd, signal);
    }

    /// <summary>EMA med adjust=False (som pandas ewm): første værdi = første kurs.</summary>
    private static decimal[] Ema(IReadOnlyList<decimal> values, int span)
    {
        var result = new decimal[values.Count];
        if (values.Count == 0) return result;

        var alpha = 2m / (span + 1);
        result[0] = values[0];
        for (var i = 1; i < values.Count; i++)
            result[i] = alpha * values[i] + (1m - alpha) * result[i - 1];
        return result;
    }
}
