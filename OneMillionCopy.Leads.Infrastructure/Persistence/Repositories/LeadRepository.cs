using Microsoft.EntityFrameworkCore;
using OneMillionCopy.Leads.Application.Abstractions.Persistence;
using OneMillionCopy.Leads.Application.Common.Models;
using OneMillionCopy.Leads.Application.Leads;
using OneMillionCopy.Leads.Application.Leads.Dtos;
using OneMillionCopy.Leads.Domain.Entities;
using OneMillionCopy.Leads.Domain.Enums;

namespace OneMillionCopy.Leads.Infrastructure.Persistence.Repositories;

public sealed class LeadRepository : ILeadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LeadRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Leads.AnyAsync(x => x.Email == email, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(Guid excludedId, string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Leads.AnyAsync(x => x.Id != excludedId && x.Email == email, cancellationToken);
    }

    public async Task AddAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        await _dbContext.Leads.AddAsync(lead, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Lead?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Leads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<LeadResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Leads
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new LeadResponse(
                x.Id,
                x.Nombre,
                x.Email,
                x.Telefono,
                x.Fuente == LeadSource.Instagram ? "instagram" :
                x.Fuente == LeadSource.Facebook ? "facebook" :
                x.Fuente == LeadSource.LandingPage ? "landing_page" :
                x.Fuente == LeadSource.Referido ? "referido" : "otro",
                x.ProductoInteres,
                x.Presupuesto,
                x.CreatedAtUtc,
                x.UpdatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<LeadStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var leads = _dbContext.Leads.AsNoTracking();
        var utcNow = DateTime.UtcNow;
        var last7Days = utcNow.AddDays(-7);

        var totalLeads = await leads.CountAsync(cancellationToken);

        var leadsPorFuenteRaw = await leads
            .GroupBy(x => x.Fuente)
            .Select(group => new
            {
                Fuente = group.Key,
                Cantidad = group.Count()
            })
            .ToListAsync(cancellationToken);

        var leadsPorFuente = leadsPorFuenteRaw
            .Select(x => new LeadSourceCountResponse(
                LeadSourceMapper.ToApiValue(x.Fuente),
                x.Cantidad))
            .ToList();

        var promedioPresupuesto = await leads
            .Where(x => x.Presupuesto.HasValue)
            .Select(x => x.Presupuesto!.Value)
            .DefaultIfEmpty(0m)
            .AverageAsync(cancellationToken);

        var leadsUltimos7Dias = await leads
            .CountAsync(x => x.CreatedAtUtc >= last7Days, cancellationToken);

        return new LeadStatsResponse(
            totalLeads,
            leadsPorFuente,
            Math.Round(promedioPresupuesto, 2),
            leadsUltimos7Dias);
    }

    public async Task<PagedResult<LeadResponse>> GetPagedAsync(
        int page,
        int limit,
        LeadSource? fuente,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Leads
            .AsNoTracking()
            .AsQueryable();

        if (fuente.HasValue)
        {
            query = query.Where(x => x.Fuente == fuente.Value);
        }

        if (fechaDesde.HasValue)
        {
            var startUtc = fechaDesde.Value.ToUniversalTime();
            query = query.Where(x => x.CreatedAtUtc >= startUtc);
        }

        if (fechaHasta.HasValue)
        {
            var endUtc = fechaHasta.Value.ToUniversalTime();
            query = query.Where(x => x.CreatedAtUtc <= endUtc);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(x => new LeadResponse(
                x.Id,
                x.Nombre,
                x.Email,
                x.Telefono,
                x.Fuente == LeadSource.Instagram ? "instagram" :
                x.Fuente == LeadSource.Facebook ? "facebook" :
                x.Fuente == LeadSource.LandingPage ? "landing_page" :
                x.Fuente == LeadSource.Referido ? "referido" : "otro",
                x.ProductoInteres,
                x.Presupuesto,
                x.CreatedAtUtc,
                x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)limit);

        return new PagedResult<LeadResponse>(items, page, limit, totalItems, totalPages);
    }
}
