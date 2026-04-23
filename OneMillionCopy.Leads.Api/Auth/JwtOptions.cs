namespace OneMillionCopy.Leads.Api.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "OneMillionCopy.Leads.Api";

    public string Audience { get; set; } = "OneMillionCopy.Leads.Client";

    public string SecretKey { get; set; } = "ChangeThisSecretKeyToARealLongValue123456789";

    public int ExpirationMinutes { get; set; } = 60;
}
