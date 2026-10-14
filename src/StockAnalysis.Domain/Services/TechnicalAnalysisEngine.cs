using StockAnalysis.Domain.Indicators;
using StockAnalysis.Domain.ValueObjects;

namespace StockAnalysis.Domain.Services;

/// <summary>Domæneservice: orkestrerer indikatorer, signaler og resumé. Stateless.</summary>
public sealed class TechnicalAnalysisEngine
{
    public TechnicalAnalysisResult Analyze(string tickerName, IReadOnlyList<PriceBar> bars, AnalysisParameters? parameters = null)
    {
        if (bars is null || bars.Count < 2)
            throw new ArgumentException("Der kræves mindst to kurser for at lave en analyse.", nameof(bars));

        var p = parameters ?? AnalysisParameters.Default;
        var closes = bars.Select(b => b.Close).ToArray();

        var rsi = RsiIndicator.Calculate(closes, p.RsiPeriod);
        var macd = MacdIndicator.Calculate(closes, p.MacdFast, p.MacdSlow, p.MacdSignal);
        var bollinger = BollingerBandsIndicator.Calculate(closes, p.BollingerPeriod, p.BollingerStdDev);
        var signals = SignalEvaluator.Evaluate(bars, rsi, macd, p.RsiOversold, p.RsiOverbought);

        return new TechnicalAnalysisResult(
            bars,
            BuildIndicatorSeries(bars, rsi, macd, bollinger),
            signals,
            SummaryBuilder.Build(tickerName, bars, rsi, macd, bollinger, signals, p));
    }

    private static IReadOnlyList<IndicatorValues> BuildIndicatorSeries(
        IReadOnlyList<PriceBar> bars, decimal?[] rsi, MacdResult macd, BollingerBandsResult bb)
    {
        var list = new List<IndicatorValues>(bars.Count);
        for (var i = 0; i < bars.Count; i++)
        {
            var histogram = macd.Macd[i] is { } m && macd.Signal[i] is { } s ? m - s : (decimal?)null;
            list.Add(new IndicatorValues(
                bars[i].Date, rsi[i], macd.Macd[i], macd.Signal[i], histogram,
                bb.Upper[i], bb.Middle[i], bb.Lower[i]));
        }
        return list;
    }
}
