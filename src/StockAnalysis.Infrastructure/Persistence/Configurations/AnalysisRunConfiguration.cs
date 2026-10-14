using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAnalysis.Domain.Entities;
using StockAnalysis.Domain.ValueObjects;

namespace StockAnalysis.Infrastructure.Persistence.Configurations;

public sealed class AnalysisRunConfiguration : IEntityTypeConfiguration<AnalysisRun>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<AnalysisRun> builder)
    {
        builder.ToTable("analysis_runs");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.TickerName).IsRequired().HasMaxLength(200);
        builder.Property(r => r.SummaryText).IsRequired().HasMaxLength(4000);

        builder.Property(r => r.Signals)
            .HasColumnType("jsonb") // skift til "text" hvis du bytter databaseprovider
            .HasConversion(
                signals => JsonSerializer.Serialize(signals, JsonOptions),
                json => JsonSerializer.Deserialize<List<TradeSignal>>(json, JsonOptions) ?? new List<TradeSignal>());

        builder.HasIndex(r => new { r.TickerId, r.CreatedAtUtc });
    }
}
