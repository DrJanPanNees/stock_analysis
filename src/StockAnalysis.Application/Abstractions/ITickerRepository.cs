using StockAnalysis.Domain.Entities;

namespace StockAnalysis.Application.Abstractions;

public interface ITickerRepository
{
    Task<IReadOnlyList<Ticker>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Ticker?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
    Task AddAsync(Ticker ticker, CancellationToken cancellationToken = default);
}
