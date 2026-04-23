using System.Text;
using OneMillionCopy.Leads.Application.Leads.Dtos;
using OneMillionCopy.Leads.Application.Leads.Services;

namespace OneMillionCopy.Leads.Infrastructure.AI;

public sealed class MockLeadSummaryGenerator : ILeadAiSummaryGenerator
{
    public Task<string> GenerateAsync(
        IReadOnlyCollection<LeadAiSummaryItem> leads,
        string? fuente,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        CancellationToken cancellationToken = default)
    {
        var total = leads.Count;
        var sourceBreakdown = leads
            .GroupBy(x => x.Fuente)
            .Select(x => new { Fuente = x.Key, Cantidad = x.Count() })
            .OrderByDescending(x => x.Cantidad)
            .ToList();

        var fuentePrincipal = sourceBreakdown.FirstOrDefault()?.Fuente ?? "sin datos";
        var promedioPresupuesto = leads.Where(x => x.Presupuesto.HasValue).Select(x => x.Presupuesto!.Value).DefaultIfEmpty(0m).Average();
        var topProducts = leads
            .Where(x => !string.IsNullOrWhiteSpace(x.ProductoInteres))
            .GroupBy(x => x.ProductoInteres!)
            .OrderByDescending(x => x.Count())
            .Take(3)
            .Select(x => x.Key)
            .ToArray();

        var sb = new StringBuilder();
        sb.AppendLine("Resumen ejecutivo generado en modo mock.");
        sb.AppendLine();
        sb.AppendLine("Analisis general:");
        sb.AppendLine($"Se analizaron {total} leads{BuildFilterSuffix(fuente, fechaDesde, fechaHasta)}. El presupuesto promedio observado es USD {Math.Round(promedioPresupuesto, 2):0.00}.");
        if (topProducts.Length > 0)
        {
            sb.AppendLine($"Los productos de mayor interes son: {string.Join(", ", topProducts)}.");
        }

        sb.AppendLine();
        sb.AppendLine("Fuente principal:");
        sb.AppendLine($"La fuente con mayor participacion es {fuentePrincipal}. Distribucion: {string.Join(", ", sourceBreakdown.Select(x => $"{x.Fuente}: {x.Cantidad}"))}.");

        sb.AppendLine();
        sb.AppendLine("Recomendaciones:");
        sb.AppendLine("Priorizar seguimiento rapido de los leads de la fuente principal.");
        sb.AppendLine("Crear mensajes comerciales diferenciados para los productos con mayor interes.");
        sb.AppendLine("Revisar los leads sin presupuesto para mejorar la calificacion comercial.");

        return Task.FromResult(sb.ToString().Trim());
    }

    private static string BuildFilterSuffix(string? fuente, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(fuente))
        {
            filters.Add($"fuente = {fuente}");
        }

        if (fechaDesde.HasValue)
        {
            filters.Add($"fecha_desde = {fechaDesde.Value:yyyy-MM-dd}");
        }

        if (fechaHasta.HasValue)
        {
            filters.Add($"fecha_hasta = {fechaHasta.Value:yyyy-MM-dd}");
        }

        return filters.Count == 0 ? string.Empty : $" con filtros ({string.Join("; ", filters)})";
    }
}
