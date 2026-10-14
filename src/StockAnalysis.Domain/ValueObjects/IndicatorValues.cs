namespace StockAnalysis.Domain.ValueObjects;

public sealed record IndicatorValues(
    DateOnly Date,
    decimal? Rsi,
    decimal? Macd,
    decimal? MacdSignal,
    decimal? MacdHistogram,
    decimal? BollingerUpper,
    decimal? BollingerMiddle,
    decimal? BollingerLower);
