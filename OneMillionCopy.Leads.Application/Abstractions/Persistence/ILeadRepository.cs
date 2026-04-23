using OneMillionCopy.Leads.Domain.Entities;
using OneMillionCopy.Leads.Domain.Enums;
using OneMillionCopy.Leads.Application.Common.Models;
using OneMillionCopy.Leads.Application.Leads.Dtos;

namespace OneMillionCopy.Leads.Application.Abstractions.Persistence;

public interface ILeadRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(Guid excludedId, string email, CancellationToken cancellationToken = default);

    Task AddAsync(Lead lead, CancellationToken cancellationToken = default);

    Task<Lead?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task UpdateAsync(CancellationToken cancellationToken = default);

    Task<LeadResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LeadStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LeadAiSummaryItem>> GetForSummaryAsync(
        LeadSource? fuente,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        CancellationToken cancellationToken = default);

    Task<PagedResult<LeadResponse>> GetPagedAsync(
        int page,
        int limit,
        LeadSource? fuente,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        CancellationToken cancellationToken = default);
}
