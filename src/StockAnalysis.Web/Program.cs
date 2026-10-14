using StockAnalysis.Application.Services;
using StockAnalysis.Domain.Services;
using StockAnalysis.Infrastructure;
using StockAnalysis.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<TechnicalAnalysisEngine>(); // stateless domæneservice
builder.Services.AddScoped<StockAnalysisService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.InitializeAsync(db);
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
