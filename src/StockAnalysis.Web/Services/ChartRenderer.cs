using ScottPlot;
using StockAnalysis.Domain.ValueObjects;

namespace StockAnalysis.Web.Services;

public static class ChartRenderer
{
    private const int Width = 1100;
    private const int Height = 450;

    public static string RenderPriceChart(TechnicalAnalysisResult result)
    {
        var plot = new Plot();

        var candlesticks = plot.Add.Candlestick(result.Bars.Select(b => new OHLC(
                (double)b.Open, (double)b.High, (double)b.Low, (double)b.Close,
                b.Date.ToDateTime(TimeOnly.MinValue), TimeSpan.FromDays(1))).ToList());
        candlesticks.RisingFillStyle.Color = Colors.Green;
        candlesticks.RisingLineStyle.Color = Colors.Green;
        candlesticks.FallingFillStyle.Color = Colors.Red;
        candlesticks.FallingLineStyle.Color = Colors.Red;

        var xs = GetXs(result);
        AddLine(plot, xs, result.Indicators.Select(i => i.BollingerMiddle), Colors.Blue, "BB Middle");
        AddLine(plot, xs, result.Indicators.Select(i => i.BollingerUpper), Colors.Green, "BB Upper");
        AddLine(plot, xs, result.Indicators.Select(i => i.BollingerLower), Colors.Red, "BB Lower");

        AddSignalMarkers(plot, result, SignalKind.Buy, Colors.Green, "Købssignal", MarkerShape.FilledCircle);
        AddSignalMarkers(plot, result, SignalKind.Sell, Colors.Red, "Salgssignal", MarkerShape.FilledSquare);

        plot.Axes.DateTimeTicksBottom();
        plot.Title($"{result.Summary.TickerName} – pris + Bollinger Bands");
        plot.ShowLegend(Alignment.UpperLeft);
        return ToBase64(plot);
    }

    public static string RenderRsiChart(TechnicalAnalysisResult result)
    {
        var plot = new Plot();
        var xs = GetXs(result);

        AddLine(plot, xs, result.Indicators.Select(i => i.Rsi), Colors.Blue, "RSI");
        AddReferenceLine(plot, xs, 70, Colors.Red, "Overbought (70)");
        AddReferenceLine(plot, xs, 30, Colors.Green, "Oversold (30)");
        AddIndicatorMarkers(plot, result, SignalKind.Buy, i => i.Rsi, Colors.Green, "Køb", MarkerShape.FilledCircle);
        AddIndicatorMarkers(plot, result, SignalKind.Sell, i => i.Rsi, Colors.Red, "Salg", MarkerShape.FilledSquare);

        plot.Axes.DateTimeTicksBottom();
        plot.Title($"{result.Summary.TickerName} – RSI");
        plot.ShowLegend(Alignment.UpperLeft);
        return ToBase64(plot);
    }

    public static string RenderMacdChart(TechnicalAnalysisResult result)
    {
        var plot = new Plot();
        var xs = GetXs(result);

        AddLine(plot, xs, result.Indicators.Select(i => i.Macd), Colors.Blue, "MACD");
        AddLine(plot, xs, result.Indicators.Select(i => i.MacdSignal), Colors.Orange, "Signal");
        plot.Add.HorizontalLine(0);
        AddIndicatorMarkers(plot, result, SignalKind.Buy, i => i.Macd, Colors.Green, "MACD køb", MarkerShape.FilledCircle);
        AddIndicatorMarkers(plot, result, SignalKind.Sell, i => i.Macd, Colors.Red, "MACD salg", MarkerShape.FilledSquare);

        plot.Axes.DateTimeTicksBottom();
        plot.Title($"{result.Summary.TickerName} – MACD");
        plot.ShowLegend(Alignment.UpperLeft);
        return ToBase64(plot);
    }

    private static double[] GetXs(TechnicalAnalysisResult result)
        => result.Indicators.Select(i => i.Date.ToDateTime(TimeOnly.MinValue).ToOADate()).ToArray();

    private static void AddLine(Plot plot, double[] xs, IEnumerable<decimal?> values, Color color, string label)
    {
        var points = xs.Zip(values, (x, v) => (X: x, V: v)).Where(p => p.V.HasValue).ToArray();
        if (points.Length == 0) return;

        var scatter = plot.Add.Scatter(
            points.Select(p => p.X).ToArray(),
            points.Select(p => (double)p.V!.Value).ToArray());
        scatter.Color = color;
        scatter.LineWidth = 1.5f;
        scatter.MarkerStyle = MarkerStyle.None;
        scatter.Label = label;
    }

    private static void AddReferenceLine(Plot plot, double[] xs, double y, Color color, string label)
    {
        if (xs.Length == 0) return;
        var scatter = plot.Add.Scatter(new[] { xs[0], xs[^1] }, new[] { y, y });
        scatter.Color = color;
        scatter.LineWidth = 1.5f;
        scatter.LinePattern = LinePattern.Dashed;
        scatter.MarkerStyle = MarkerStyle.None;
        scatter.Label = label;
    }

    private static void AddSignalMarkers(Plot plot, TechnicalAnalysisResult result, SignalKind kind, Color color, string label, MarkerShape shape)
    {
        var points = result.Signals
            .Where(s => s.Kind == kind)
            .Select(s => (X: s.Date.ToDateTime(TimeOnly.MinValue).ToOADate(), Y: (double)s.Close))
            .ToArray();
        AddMarkers(plot, points, color, label, shape);
    }

    private static void AddIndicatorMarkers(Plot plot, TechnicalAnalysisResult result, SignalKind kind,
        Func<IndicatorValues, decimal?> yOf, Color color, string label, MarkerShape shape)
    {
        var byDate = result.Indicators.ToDictionary(i => i.Date);
        var points = result.Signals
            .Where(s => s.Kind == kind && byDate.ContainsKey(s.Date) && yOf(byDate[s.Date]).HasValue)
            .Select(s => (X: byDate[s.Date].Date.ToDateTime(TimeOnly.MinValue).ToOADate(),
                          Y: (double)yOf(byDate[s.Date])!.Value))
            .ToArray();
        AddMarkers(plot, points, color, label, shape);
    }

    private static void AddMarkers(Plot plot, (double X, double Y)[] points, Color color, string label, MarkerShape shape)
    {
        if (points.Length == 0) return;
        var scatter = plot.Add.Scatter(points.Select(p => p.X).ToArray(), points.Select(p => p.Y).ToArray());
        scatter.Color = color;
        scatter.LineWidth = 0;
        scatter.MarkerStyle = new MarkerStyle(shape, 10, color);
        scatter.Label = label;
    }

    private static string ToBase64(Plot plot)
        => "data:image/png;base64," + Convert.ToBase64String(plot.GetImageBytes(Width, Height, ImageFormat.Png));
}
