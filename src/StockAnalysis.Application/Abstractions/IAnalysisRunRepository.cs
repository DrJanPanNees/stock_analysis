using StockAnalysis.Domain.Entities;

namespace StockAnalysis.Application.Abstractions;

public interface IAnalysisRunRepository
{
    Task AddAsync(AnalysisRun analysisRun, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AnalysisRun>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
}
