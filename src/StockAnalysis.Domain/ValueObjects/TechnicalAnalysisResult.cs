namespace StockAnalysis.Domain.ValueObjects;

public sealed record TechnicalAnalysisResult(
    IReadOnlyList<PriceBar> Bars,
    IReadOnlyList<IndicatorValues> Indicators,
    IReadOnlyList<TradeSignal> Signals,
    AnalysisSummary Summary);
