using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAnalysis.Domain.Entities;

namespace StockAnalysis.Infrastructure.Persistence.Configurations;

public sealed class TickerConfiguration : IEntityTypeConfiguration<Ticker>
{
    public void Configure(EntityTypeBuilder<Ticker> builder)
    {
        builder.ToTable("tickers");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Symbol).IsRequired().HasMaxLength(32);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(t => t.Symbol).IsUnique();
    }
}
