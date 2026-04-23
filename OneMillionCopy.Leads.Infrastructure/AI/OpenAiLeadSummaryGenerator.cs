using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OneMillionCopy.Leads.Application.Leads.Dtos;
using OneMillionCopy.Leads.Application.Leads.Services;

namespace OneMillionCopy.Leads.Infrastructure.AI;

public sealed class OpenAiLeadSummaryGenerator : ILeadAiSummaryGenerator
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiOptions _options;

    public OpenAiLeadSummaryGenerator(HttpClient httpClient, IOptions<OpenAiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> GenerateAsync(
        IReadOnlyCollection<LeadAiSummaryItem> leads,
        string? fuente,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "responses");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        var payload = new
        {
            model = _options.Model,
            instructions = "Eres un analista comercial senior. Genera un resumen ejecutivo claro en espanol con tres secciones: Analisis general, Fuente principal y Recomendaciones. No inventes datos. Usa solo la informacion entregada.",
            input = BuildPrompt(leads, fuente, fechaDesde, fechaHasta)
        };

        request.Content = JsonContent.Create(payload);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OpenAI devolvio un error: {response.StatusCode}. Detalle: {responseText}");
        }

        using var document = JsonDocument.Parse(responseText);
        var outputText = TryReadOutputText(document.RootElement);

        if (string.IsNullOrWhiteSpace(outputText))
        {
            throw new InvalidOperationException("No fue posible obtener el texto del resumen desde la respuesta de OpenAI.");
        }

        return outputText.Trim();
    }

    private static string BuildPrompt(
        IReadOnlyCollection<LeadAiSummaryItem> leads,
        string? fuente,
        DateTime? fechaDesde,
        DateTime? fechaHasta)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Genera un resumen ejecutivo de leads.");
        sb.AppendLine($"Filtro fuente: {fuente ?? "sin filtro"}");
        sb.AppendLine($"Filtro fecha_desde: {(fechaDesde.HasValue ? fechaDesde.Value.ToString("O") : "sin filtro")}");
        sb.AppendLine($"Filtro fecha_hasta: {(fechaHasta.HasValue ? fechaHasta.Value.ToString("O") : "sin filtro")}");
        sb.AppendLine($"Cantidad de leads: {leads.Count}");
        sb.AppendLine("Leads:");

        foreach (var lead in leads)
        {
            sb.AppendLine(JsonSerializer.Serialize(lead));
        }

        return sb.ToString();
    }

    private static string? TryReadOutputText(JsonElement root)
    {
        if (root.TryGetProperty("output_text", out var outputTextProperty) &&
            outputTextProperty.ValueKind == JsonValueKind.String)
        {
            return outputTextProperty.GetString();
        }

        if (!root.TryGetProperty("output", out var output) || output.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var fragments = new List<string>();

        foreach (var item in output.EnumerateArray())
        {
            if (!item.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var contentItem in content.EnumerateArray())
            {
                if (contentItem.TryGetProperty("text", out var textProperty) &&
                    textProperty.ValueKind == JsonValueKind.String)
                {
                    fragments.Add(textProperty.GetString()!);
                }
            }
        }

        return fragments.Count == 0 ? null : string.Join(Environment.NewLine, fragments);
    }
}
