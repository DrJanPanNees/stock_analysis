using Microsoft.EntityFrameworkCore;
using StockAnalysis.Domain.Entities;

namespace StockAnalysis.Infrastructure.Persistence;

public static class DbInitializer
{
    private static readonly (string Symbol, string Name, bool IsActive)[] SeedTickers =
    {
        ("MEUD.L", "Amundi Stoxx Europe 600 UCITS ETF Acc", false),
        ("SPY5.L", "SPDR S&P 500 ETF", false),
        ("VHYL.L", "Vanguard High Dividend Yield ETF", false),
        ("VWCE.DE", "VANGUARD FTSE ALL WORLD", false),
        ("QUBT", "Quantum Computing", false),
        ("0P0001M5YR.CO", "Nordnet Tech - genkøb?", false),
        ("NUKL.DE", "VanEck Nuclear", false),
        ("DAPP.L", "VanEck Crypto", false),
        ("GDX.L", "VanEck Gold Miners UCITS ETF", true),
        ("EXSH.DE", "Martin: SToxx Europe Select Dividend 30", false),
        ("AIQU.L", "GOBAL X ARTI ETF USD", false),
    };

    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync(); // til produktion: brug migrations i stedet

        if (await context.Tickers.AnyAsync()) return;

        foreach (var (symbol, name, isActive) in SeedTickers)
            context.Tickers.Add(Ticker.Create(symbol, name, isActive));

        await context.SaveChangesAsync();
    }
}
