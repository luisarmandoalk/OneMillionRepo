namespace OneMillionCopy.Leads.Application.Leads.Commands.CreateLead;

public sealed record CreateLeadCommand(
    string Nombre,
    string Email,
    string? Telefono,
    string Fuente,
    string? ProductoInteres,
    decimal? Presupuesto);
