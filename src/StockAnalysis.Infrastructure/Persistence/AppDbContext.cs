using Microsoft.EntityFrameworkCore;
using StockAnalysis.Domain.Entities;

namespace StockAnalysis.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Ticker> Tickers => Set<Ticker>();
    public DbSet<AnalysisRun> AnalysisRuns => Set<AnalysisRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
