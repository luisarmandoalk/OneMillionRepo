using OneMillionCopy.Leads.Domain.Common;
using OneMillionCopy.Leads.Domain.Enums;

namespace OneMillionCopy.Leads.Domain.Entities;

public sealed class Lead : BaseAuditableEntity
{
    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public LeadSource Fuente { get; set; }

    public string? ProductoInteres { get; set; }

    public decimal? Presupuesto { get; set; }
}
