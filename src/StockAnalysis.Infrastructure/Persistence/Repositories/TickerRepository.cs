using Microsoft.EntityFrameworkCore;
using StockAnalysis.Application.Abstractions;
using StockAnalysis.Domain.Entities;

namespace StockAnalysis.Infrastructure.Persistence.Repositories;

public sealed class TickerRepository : ITickerRepository
{
    private readonly AppDbContext _db;
    public TickerRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Ticker>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _db.Tickers.AsNoTracking().OrderBy(t => t.Name).ToListAsync(cancellationToken);

    public async Task<Ticker?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.Tickers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<bool> ExistsBySymbolAsync(string symbol, CancellationToken cancellationToken = default)
        => await _db.Tickers.AnyAsync(t => t.Symbol == symbol, cancellationToken);

    public async Task AddAsync(Ticker ticker, CancellationToken cancellationToken = default)
    {
        _db.Tickers.Add(ticker);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

public sealed class AnalysisRunRepository : IAnalysisRunRepository
{
    private readonly AppDbContext _db;
    public AnalysisRunRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(AnalysisRun analysisRun, CancellationToken cancellationToken = default)
    {
        _db.AnalysisRuns.Add(analysisRun);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AnalysisRun>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
        => await _db.AnalysisRuns.AsNoTracking()
            .OrderByDescending(r => r.CreatedAtUtc)
            .Take(count)
            .ToListAsync(cancellationToken);
}
