using StockAnalysis.Domain.ValueObjects;

namespace StockAnalysis.Application.Abstractions;

/// <summary>Port til markeddata – implementeres som adapter i Infrastructure.</summary>
public interface IMarketDataProvider
{
    Task<IReadOnlyList<PriceBar>> GetDailyHistoryAsync(string symbol, int months, CancellationToken cancellationToken = default);
}
