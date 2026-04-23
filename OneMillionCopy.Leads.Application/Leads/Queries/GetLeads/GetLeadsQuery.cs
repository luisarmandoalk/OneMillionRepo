namespace OneMillionCopy.Leads.Application.Leads.Queries.GetLeads;

public sealed record GetLeadsQuery(
    int Page = 1,
    int Limit = 10,
    string? Fuente = null,
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null);
