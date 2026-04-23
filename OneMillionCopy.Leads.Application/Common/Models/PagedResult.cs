namespace OneMillionCopy.Leads.Application.Common.Models;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int Limit,
    int TotalItems,
    int TotalPages);
