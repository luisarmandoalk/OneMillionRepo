namespace OneMillionCopy.Leads.Application.Leads.Commands.GenerateLeadSummary;

public sealed record GenerateLeadSummaryCommand(
    string? Fuente,
    DateTime? FechaDesde,
    DateTime? FechaHasta);
