namespace StockAnalysis.Domain.Entities;

public sealed class Ticker
{
    public Guid Id { get; private set; }
    public string Symbol { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private Ticker() { } // EF Core

    private Ticker(Guid id, string symbol, string name, bool isActive)
    {
        Id = id; Symbol = symbol; Name = name; IsActive = isActive;
    }

    public static Ticker Create(string symbol, string name, bool isActive = true)
        => new(Guid.NewGuid(), symbol.Trim(), name.Trim(), isActive);
}
