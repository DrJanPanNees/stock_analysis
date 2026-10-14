using StockAnalysis.Domain.ValueObjects;

namespace StockAnalysis.Domain.Indicators;

public static class BollingerBandsIndicator
{
    public static BollingerBandsResult Calculate(IReadOnlyList<decimal> closes, int period, decimal stdDevMultiplier)
    {
        var middle = new decimal?[closes.Count];
        var upper = new decimal?[closes.Count];
        var lower = new decimal?[closes.Count];

        if (closes.Count < period || period < 2)
            return new BollingerBandsResult(middle, upper, lower);

        for (var i = period - 1; i < closes.Count; i++)
        {
            decimal sum = 0m;
            for (var j = i - period + 1; j <= i; j++) sum += closes[j];
            var mean = sum / period;

            decimal sqSum = 0m;
            for (var j = i - period + 1; j <= i; j++)
            {
                var diff = closes[j] - mean;
                sqSum += diff * diff;
            }

            var std = (decimal)Math.Sqrt((double)(sqSum / (period - 1))); // pandas .std() bruger ddof=1

            middle[i] = mean;
            upper[i] = mean + stdDevMultiplier * std;
            lower[i] = mean - stdDevMultiplier * std;
        }
        return new BollingerBandsResult(middle, upper, lower);
    }
}
