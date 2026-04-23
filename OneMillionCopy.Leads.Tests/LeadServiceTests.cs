using OneMillionCopy.Leads.Application.Abstractions.Persistence;
using OneMillionCopy.Leads.Application.Common.Exceptions;
using OneMillionCopy.Leads.Application.Common.Models;
using OneMillionCopy.Leads.Application.Leads.Commands.CreateLead;
using OneMillionCopy.Leads.Application.Leads.Commands.GenerateLeadSummary;
using OneMillionCopy.Leads.Application.Leads.Dtos;
using OneMillionCopy.Leads.Application.Leads.Services;
using OneMillionCopy.Leads.Domain.Entities;
using OneMillionCopy.Leads.Domain.Enums;

namespace OneMillionCopy.Leads.Tests;

public sealed class LeadServiceTests
{
    [Fact]
    public async Task CreateLeadCommandOK()
    {
        var repository = new FakeLeadRepository();
        var aiSummaryGenerator = new FakeLeadAiSummaryGenerator();
        var service = new LeadService(repository, aiSummaryGenerator);

        var command = new CreateLeadCommand(
            "Luisa Gomez",
            "luisa@example.com",
            "3001234567",
            "instagram",
            "Curso de copywriting",
            150m);

        var response = await service.CreateAsync(command);

        Assert.Equal("Luisa Gomez", response.Nombre);
        Assert.Equal("luisa@example.com", response.Email);
        Assert.Equal("instagram", response.Fuente);
        Assert.Equal(150m, response.Presupuesto);
        Assert.Single(repository.StoredLeads);
    }

    [Fact]
    public async Task LeadServiceEmailNullError()
    {
        var repository = new FakeLeadRepository
        {
            EmailExistsResult = true
        };
        var aiSummaryGenerator = new FakeLeadAiSummaryGenerator();
        var service = new LeadService(repository, aiSummaryGenerator);

        var command = new CreateLeadCommand(
            "Luisa Gomez",
            "luisa@example.com",
            null,
            "facebook",
            null,
            null);

        var exception = await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(command));

        Assert.Equal("Ya existe un lead registrado con ese email.", exception.Message);
    }

    [Fact]
    public async Task GenerateLeadSummaryCommandDateRangeIsInvalidError()
    {
        var repository = new FakeLeadRepository();
        var aiSummaryGenerator = new FakeLeadAiSummaryGenerator();
        var service = new LeadService(repository, aiSummaryGenerator);

        var command = new GenerateLeadSummaryCommand(
            "instagram",
            new DateTime(2026, 04, 23),
            new DateTime(2026, 04, 01));

        var exception = await Assert.ThrowsAsync<ValidationException>(() => service.GenerateSummaryAsync(command));

        Assert.Equal("El rango de fechas no es valido: fecha_desde no puede ser mayor que fecha_hasta.", exception.Message);
    }

    private sealed class FakeLeadRepository : ILeadRepository
    {
        public bool EmailExistsResult { get; set; }

        public List<Lead> StoredLeads { get; } = [];

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(EmailExistsResult);
        }

        public Task<bool> EmailExistsAsync(Guid excludedId, string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task AddAsync(Lead lead, CancellationToken cancellationToken = default)
        {
            StoredLeads.Add(lead);
            return Task.CompletedTask;
        }

        public Task<Lead?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Lead?>(StoredLeads.FirstOrDefault(x => x.Id == id));
        }

        public Task UpdateAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<LeadResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<LeadResponse?>(null);
        }

        public Task<LeadStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new LeadStatsResponse(0, [], 0m, 0));
        }

        public Task<IReadOnlyCollection<LeadAiSummaryItem>> GetForSummaryAsync(
            LeadSource? fuente,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<LeadAiSummaryItem>>([]);
        }

        public Task<PagedResult<LeadResponse>> GetPagedAsync(
            int page,
            int limit,
            LeadSource? fuente,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedResult<LeadResponse>([], page, limit, 0, 0));
        }
    }

    private sealed class FakeLeadAiSummaryGenerator : ILeadAiSummaryGenerator
    {
        public Task<string> GenerateAsync(
            IReadOnlyCollection<LeadAiSummaryItem> leads,
            string? fuente,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult("Resumen mock");
        }
    }
}
