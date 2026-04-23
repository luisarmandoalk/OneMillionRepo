using System.Text.Json;

namespace OneMillionCopy.Leads.Api.Contracts.Leads;

public sealed class UpdateLeadRequest
{
    public JsonElement? Nombre { get; set; }

    public JsonElement? Email { get; set; }

    public JsonElement? Telefono { get; set; }

    public JsonElement? Fuente { get; set; }

    public JsonElement? ProductoInteres { get; set; }

    public JsonElement? Presupuesto { get; set; }
}
