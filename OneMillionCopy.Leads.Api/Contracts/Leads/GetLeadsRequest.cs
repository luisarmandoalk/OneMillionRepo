namespace OneMillionCopy.Leads.Api.Contracts.Leads;

public sealed class GetLeadsRequest
{
    public int Page { get; set; } = 1;

    public int Limit { get; set; } = 10;

    public string? Fuente { get; set; }

    public DateTime? FechaDesde { get; set; }

    public DateTime? FechaHasta { get; set; }
}
