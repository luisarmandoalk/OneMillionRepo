namespace OneMillionCopy.Leads.Application.Leads.Dtos;

public sealed record LeadStatsResponse(
    int TotalLeads,
    IReadOnlyCollection<LeadSourceCountResponse> LeadsPorFuente,
    decimal PromedioPresupuesto,
    int LeadsUltimos7Dias);
