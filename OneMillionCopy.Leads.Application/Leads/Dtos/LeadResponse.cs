namespace OneMillionCopy.Leads.Application.Leads.Dtos;

public sealed record LeadResponse(
    Guid Id,
    string Nombre,
    string Email,
    string? Telefono,
    string Fuente,
    string? ProductoInteres,
    decimal? Presupuesto,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
