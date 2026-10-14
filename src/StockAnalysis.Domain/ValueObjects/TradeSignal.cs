namespace StockAnalysis.Domain.ValueObjects;

public enum SignalKind { Buy, Sell }
public enum SignalSource { Rsi, Macd }

public sealed record TradeSignal(
    DateOnly Date,
    SignalKind Kind,
    SignalSource Source,
    decimal Close);
