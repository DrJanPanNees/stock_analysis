namespace StockAnalysis.Domain.Indicators;

public static class RsiIndicator
{
    public static decimal?[] Calculate(IReadOnlyList<decimal> closes, int period)
    {
        var result = new decimal?[closes.Count];
        if (closes.Count < period) return result;

        decimal gainSum = 0m, lossSum = 0m;
        for (var i = 1; i < period; i++)
        {
            var delta = closes[i] - closes[i - 1];
            if (delta > 0m) gainSum += delta; else lossSum -= delta;
        }
        result[period - 1] = ComputeRsi(gainSum, lossSum);

        for (var i = period; i < closes.Count; i++)
        {
            var newDelta = closes[i] - closes[i - 1];
            var oldDelta = i == period ? 0m : closes[i - period] - closes[i - period - 1];

            gainSum += Math.Max(newDelta, 0m) - Math.Max(oldDelta, 0m);
            lossSum += Math.Max(-newDelta, 0m) - Math.Max(-oldDelta, 0m);

            result[i] = ComputeRsi(gainSum, lossSum);
        }
        return result;
    }

    private static decimal? ComputeRsi(decimal gainSum, decimal lossSum)
    {
        if (lossSum == 0m) return gainSum == 0m ? null : 100m; // 0/0 => NaN i pandas
        var rs = gainSum / lossSum;
        return 100m - 100m / (1m + rs);
    }
}
