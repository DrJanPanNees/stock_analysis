namespace StockAnalysis.Domain.ValueObjects;

public sealed record MacdResult(decimal?[] Macd, decimal?[] Signal);

public sealed record BollingerBandsResult(decimal?[] Middle, decimal?[] Upper, decimal?[] Lower);
