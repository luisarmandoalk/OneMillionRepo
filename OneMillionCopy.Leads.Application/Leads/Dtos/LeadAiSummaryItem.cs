namespace OneMillionCopy.Leads.Application.Leads.Dtos;

public sealed record LeadAiSummaryItem(
    Guid Id,
    string Nombre,
    string Email,
    string? Telefono,
    string Fuente,
    string? ProductoInteres,
    decimal? Presupuesto,
    DateTime CreatedAtUtc);
