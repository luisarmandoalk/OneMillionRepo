namespace OneMillionCopy.Leads.Api.Auth;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public string Username { get; set; } = "admin";

    public string Password { get; set; } = "Admin123*";
}
