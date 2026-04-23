using OneMillionCopy.Leads.Application.Leads.Commands.CreateLead;
using OneMillionCopy.Leads.Application.Leads.Dtos;
using OneMillionCopy.Leads.Application.Common.Models;
using OneMillionCopy.Leads.Application.Leads.Queries.GetLeads;
using OneMillionCopy.Leads.Application.Leads.Commands.UpdateLead;
using OneMillionCopy.Leads.Application.Leads.Commands.GenerateLeadSummary;

namespace OneMillionCopy.Leads.Application.Leads.Services;

public interface ILeadService
{
    Task<LeadResponse> CreateAsync(CreateLeadCommand command, CancellationToken cancellationToken = default);

    Task<LeadResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LeadResponse> UpdateAsync(UpdateLeadCommand command, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LeadStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default);

    Task<string> GenerateSummaryAsync(GenerateLeadSummaryCommand command, CancellationToken cancellationToken = default);

    Task<PagedResult<LeadResponse>> GetPagedAsync(GetLeadsQuery query, CancellationToken cancellationToken = default);
}
