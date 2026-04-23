using System.ComponentModel.DataAnnotations;

namespace OneMillionCopy.Leads.Api.Contracts.Auth;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "El campo username es obligatorio.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo password es obligatorio.")]
    public string Password { get; set; } = string.Empty;
}
