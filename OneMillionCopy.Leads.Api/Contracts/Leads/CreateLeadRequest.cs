using System.ComponentModel.DataAnnotations;

namespace OneMillionCopy.Leads.Api.Contracts.Leads;

public sealed class CreateLeadRequest
{
    [Required(ErrorMessage = "El campo nombre es obligatorio.")]
    [MinLength(2, ErrorMessage = "El campo nombre debe tener al menos 2 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo email debe tener un formato valido.")]
    public string Email { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    [Required(ErrorMessage = "El campo fuente es obligatorio.")]
    public string Fuente { get; set; } = string.Empty;

    public string? ProductoInteres { get; set; }

    public decimal? Presupuesto { get; set; }
}
