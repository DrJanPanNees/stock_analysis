using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockAnalysis.Application.Abstractions;
using StockAnalysis.Infrastructure.MarketData;
using StockAnalysis.Infrastructure.Persistence;
using StockAnalysis.Infrastructure.Persistence.Repositories;

namespace StockAnalysis.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("StockAnalysis")));

        services.AddScoped<ITickerRepository, TickerRepository>();
        services.AddScoped<IAnalysisRunRepository, AnalysisRunRepository>();
        services.AddScoped<IMarketDataProvider, YahooMarketDataProvider>();

        return services;
    }
}
