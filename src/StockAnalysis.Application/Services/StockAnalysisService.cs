using StockAnalysis.Application.Abstractions;
using StockAnalysis.Domain.Entities;
using StockAnalysis.Domain.Services;
using StockAnalysis.Domain.ValueObjects;

namespace StockAnalysis.Application.Services;

/// <summary>Use case: hent markeddata, kør domæneanalyse, gem resultatet.</summary>
public sealed class StockAnalysisService
{
    private readonly ITickerRepository _tickerRepository;
    private readonly IAnalysisRunRepository _analysisRunRepository;
    private readonly IMarketDataProvider _marketDataProvider;
    private readonly TechnicalAnalysisEngine _analysisEngine;

    public StockAnalysisService(
        ITickerRepository tickerRepository,
        IAnalysisRunRepository analysisRunRepository,
        IMarketDataProvider marketDataProvider,
        TechnicalAnalysisEngine analysisEngine)
    {
        _tickerRepository = tickerRepository;
        _analysisRunRepository = analysisRunRepository;
        _marketDataProvider = marketDataProvider;
        _analysisEngine = analysisEngine;
    }

    public Task<IReadOnlyList<Ticker>> GetWatchlistAsync(CancellationToken cancellationToken = default)
        => _tickerRepository.GetAllAsync(cancellationToken);

    /// <summary>Tilføjer en ticker til watchlisten. Returnerer en fejlbesked, eller null ved succes.</summary>
    public async Task<string?> AddTickerAsync(string symbol, string name, CancellationToken cancellationToken = default)
    {
        var trimmedSymbol = symbol.Trim();
        var trimmedName = name.Trim();

        if (string.IsNullOrWhiteSpace(trimmedSymbol))
            return "Symbol må ikke være tomt.";
        if (string.IsNullOrWhiteSpace(trimmedName))
            return "Navn må ikke være tomt.";

        if (await _tickerRepository.ExistsBySymbolAsync(trimmedSymbol, cancellationToken))
            return $"Tickeren '{trimmedSymbol}' findes allerede i watchlisten.";

        await _tickerRepository.AddAsync(Ticker.Create(trimmedSymbol, trimmedName), cancellationToken);
        return null;
    }

    public async Task<TechnicalAnalysisResult?> AnalyzeAsync(Guid tickerId, int months = 12, CancellationToken cancellationToken = default)
    {
        var ticker = await _tickerRepository.GetAsync(tickerId, cancellationToken);
        if (ticker is null) return null;

        var bars = await _marketDataProvider.GetDailyHistoryAsync(ticker.Symbol, months, cancellationToken);
        if (bars.Count < 2) return null;

        var result = _analysisEngine.Analyze(ticker.Name, bars);

        await _analysisRunRepository.AddAsync(AnalysisRun.FromResult(ticker, result), cancellationToken);
        return result;
    }

    public Task<IReadOnlyList<AnalysisRun>> GetRecentRunsAsync(int count = 10, CancellationToken cancellationToken = default)
        => _analysisRunRepository.GetRecentAsync(count, cancellationToken);
}
