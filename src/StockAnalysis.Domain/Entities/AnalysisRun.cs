using StockAnalysis.Domain.ValueObjects;

namespace StockAnalysis.Domain.Entities;

public sealed class AnalysisRun
{
    public Guid Id { get; private set; }
    public Guid TickerId { get; private set; }
    public string TickerName { get; private set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateOnly LastBarDate { get; private set; }
    public decimal LastClose { get; private set; }
    public decimal? Rsi { get; private set; }
    public decimal? Macd { get; private set; }
    public decimal? MacdSignal { get; private set; }
    public decimal? BollingerUpper { get; private set; }
    public decimal? BollingerMiddle { get; private set; }
    public decimal? BollingerLower { get; private set; }

    public string SummaryText { get; private set; } = null!;
    public List<TradeSignal> Signals { get; private set; } = new();

    private AnalysisRun() { } // EF Core

    public static AnalysisRun FromResult(Ticker ticker, TechnicalAnalysisResult result, DateTimeOffset? createdAtUtc = null)
    {
        var last = result.Indicators[^1];

        return new AnalysisRun
        {
            Id = Guid.NewGuid(),
            TickerId = ticker.Id,
            TickerName = ticker.Name,
            CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow,
            LastBarDate = last.Date,
            LastClose = result.Bars[^1].Close,
            Rsi = last.Rsi,
            Macd = last.Macd,
            MacdSignal = last.MacdSignal,
            BollingerUpper = last.BollingerUpper,
            BollingerMiddle = last.BollingerMiddle,
            BollingerLower = last.BollingerLower,
            SummaryText = result.Summary.ToText(),
            Signals = result.Signals.ToList(),
        };
    }
}
