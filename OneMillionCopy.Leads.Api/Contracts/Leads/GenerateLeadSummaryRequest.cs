namespace OneMillionCopy.Leads.Api.Contracts.Leads;

public sealed class GenerateLeadSummaryRequest
{
    public string? Fuente { get; set; }

    public DateTime? FechaDesde { get; set; }

    public DateTime? FechaHasta { get; set; }
}
