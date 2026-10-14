namespace StockAnalysis.Domain.ValueObjects;

public sealed record AnalysisParameters
{
    public int RsiPeriod { get; init; } = 14;
    public decimal RsiOversold { get; init; } = 30m;
    public decimal RsiOverbought { get; init; } = 70m;
    public int MacdFast { get; init; } = 12;
    public int MacdSlow { get; init; } = 26;
    public int MacdSignal { get; init; } = 9;
    public int BollingerPeriod { get; init; } = 20;
    public decimal BollingerStdDev { get; init; } = 2m;

    public static AnalysisParameters Default { get; } = new();
}
