namespace OneMillionCopy.Leads.Api.Contracts.Auth;

public sealed record LoginResponse(
    string AccessToken,
    string TokenType,
    int ExpiresInSeconds);
