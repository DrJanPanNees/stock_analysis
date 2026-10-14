namespace StockAnalysis.Domain.ValueObjects;

/// <summary>Én dags kursdata (OHLCV).</summary>
public sealed record PriceBar(
    DateOnly Date,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    long Volume);
