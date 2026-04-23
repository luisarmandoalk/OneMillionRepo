using OneMillionCopy.Leads.Application.Leads.Dtos;

namespace OneMillionCopy.Leads.Application.Leads.Services;

public interface ILeadAiSummaryGenerator
{
    Task<string> GenerateAsync(
        IReadOnlyCollection<LeadAiSummaryItem> leads,
        string? fuente,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        CancellationToken cancellationToken = default);
}
