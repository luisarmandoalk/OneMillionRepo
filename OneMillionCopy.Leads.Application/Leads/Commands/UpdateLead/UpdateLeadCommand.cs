namespace OneMillionCopy.Leads.Application.Leads.Commands.UpdateLead;

public sealed record UpdateLeadCommand(
    Guid Id,
    bool HasNombre,
    string? Nombre,
    bool HasEmail,
    string? Email,
    bool HasTelefono,
    string? Telefono,
    bool HasFuente,
    string? Fuente,
    bool HasProductoInteres,
    string? ProductoInteres,
    bool HasPresupuesto,
    decimal? Presupuesto);
