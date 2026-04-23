using OneMillionCopy.Leads.Application.Abstractions.Persistence;
using OneMillionCopy.Leads.Application.Common.Exceptions;
using OneMillionCopy.Leads.Application.Common.Models;
using OneMillionCopy.Leads.Application.Leads;
using OneMillionCopy.Leads.Application.Leads.Commands.CreateLead;
using OneMillionCopy.Leads.Application.Leads.Commands.GenerateLeadSummary;
using OneMillionCopy.Leads.Application.Leads.Commands.UpdateLead;
using OneMillionCopy.Leads.Application.Leads.Dtos;
using OneMillionCopy.Leads.Application.Leads.Queries.GetLeads;
using OneMillionCopy.Leads.Domain.Entities;
using OneMillionCopy.Leads.Domain.Enums;

namespace OneMillionCopy.Leads.Application.Leads.Services;

public sealed class LeadService : ILeadService
{
    private static readonly Dictionary<string, LeadSource> AllowedSources = new(StringComparer.OrdinalIgnoreCase)
    {
        ["instagram"] = LeadSource.Instagram,
        ["facebook"] = LeadSource.Facebook,
        ["landing_page"] = LeadSource.LandingPage,
        ["referido"] = LeadSource.Referido,
        ["otro"] = LeadSource.Otro
    };

    private readonly ILeadRepository _leadRepository;
    private readonly ILeadAiSummaryGenerator _leadAiSummaryGenerator;

    public LeadService(ILeadRepository leadRepository, ILeadAiSummaryGenerator leadAiSummaryGenerator)
    {
        _leadRepository = leadRepository;
        _leadAiSummaryGenerator = leadAiSummaryGenerator;
    }

    public async Task<LeadResponse> CreateAsync(CreateLeadCommand command, CancellationToken cancellationToken = default)
    {
        var nombre = command.Nombre.Trim();
        var email = command.Email.Trim().ToLowerInvariant();
        var telefono = string.IsNullOrWhiteSpace(command.Telefono) ? null : command.Telefono.Trim();
        var productoInteres = string.IsNullOrWhiteSpace(command.ProductoInteres) ? null : command.ProductoInteres.Trim();

        if (nombre.Length < 2)
        {
            throw new ValidationException("El campo nombre es obligatorio y debe tener al menos 2 caracteres.");
        }

        if (!AllowedSources.TryGetValue(command.Fuente.Trim(), out var fuente))
        {
            throw new ValidationException("El campo fuente debe ser uno de estos valores: instagram, facebook, landing_page, referido, otro.");
        }

        if (command.Presupuesto is < 0)
        {
            throw new ValidationException("El presupuesto no puede ser negativo.");
        }

        if (await _leadRepository.EmailExistsAsync(email, cancellationToken))
        {
            throw new ConflictException("Ya existe un lead registrado con ese email.");
        }

        var utcNow = DateTime.UtcNow;

        var lead = new Lead
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            Email = email,
            Telefono = telefono,
            Fuente = fuente,
            ProductoInteres = productoInteres,
            Presupuesto = command.Presupuesto,
            CreatedAtUtc = utcNow,
            UpdatedAtUtc = utcNow
        };

        await _leadRepository.AddAsync(lead, cancellationToken);

        return new LeadResponse(
            lead.Id,
            lead.Nombre,
            lead.Email,
            lead.Telefono,
            LeadSourceMapper.ToApiValue(lead.Fuente),
            lead.ProductoInteres,
            lead.Presupuesto,
            lead.CreatedAtUtc,
            lead.UpdatedAtUtc);
    }

    public async Task<LeadResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lead = await _leadRepository.GetByIdAsync(id, cancellationToken);

        if (lead is null)
        {
            throw new NotFoundException("No se encontro un lead con el id indicado.");
        }

        return lead;
    }

    public async Task<LeadResponse> UpdateAsync(UpdateLeadCommand command, CancellationToken cancellationToken = default)
    {
        if (!command.HasNombre &&
            !command.HasEmail &&
            !command.HasTelefono &&
            !command.HasFuente &&
            !command.HasProductoInteres &&
            !command.HasPresupuesto)
        {
            throw new ValidationException("Debes enviar al menos un campo para actualizar.");
        }

        var lead = await _leadRepository.GetTrackedByIdAsync(command.Id, cancellationToken);

        if (lead is null)
        {
            throw new NotFoundException("No se encontro un lead con el id indicado.");
        }

        if (command.HasNombre)
        {
            var nombre = command.Nombre?.Trim() ?? string.Empty;

            if (nombre.Length < 2)
            {
                throw new ValidationException("El campo nombre es obligatorio y debe tener al menos 2 caracteres.");
            }

            lead.Nombre = nombre;
        }

        if (command.HasEmail)
        {
            var email = command.Email?.Trim().ToLowerInvariant() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ValidationException("El campo email es obligatorio.");
            }

            if (!IsValidEmail(email))
            {
                throw new ValidationException("El campo email debe tener un formato valido.");
            }

            if (await _leadRepository.EmailExistsAsync(lead.Id, email, cancellationToken))
            {
                throw new ConflictException("Ya existe un lead registrado con ese email.");
            }

            lead.Email = email;
        }

        if (command.HasTelefono)
        {
            lead.Telefono = string.IsNullOrWhiteSpace(command.Telefono) ? null : command.Telefono.Trim();
        }

        if (command.HasFuente)
        {
            if (!AllowedSources.TryGetValue(command.Fuente?.Trim() ?? string.Empty, out var fuente))
            {
                throw new ValidationException("El campo fuente debe ser uno de estos valores: instagram, facebook, landing_page, referido, otro.");
            }

            lead.Fuente = fuente;
        }

        if (command.HasProductoInteres)
        {
            lead.ProductoInteres = string.IsNullOrWhiteSpace(command.ProductoInteres) ? null : command.ProductoInteres.Trim();
        }

        if (command.HasPresupuesto)
        {
            if (command.Presupuesto is < 0)
            {
                throw new ValidationException("El presupuesto no puede ser negativo.");
            }

            lead.Presupuesto = command.Presupuesto;
        }

        await _leadRepository.UpdateAsync(cancellationToken);

        return new LeadResponse(
            lead.Id,
            lead.Nombre,
            lead.Email,
            lead.Telefono,
            LeadSourceMapper.ToApiValue(lead.Fuente),
            lead.ProductoInteres,
            lead.Presupuesto,
            lead.CreatedAtUtc,
            lead.UpdatedAtUtc);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lead = await _leadRepository.GetTrackedByIdAsync(id, cancellationToken);

        if (lead is null)
        {
            throw new NotFoundException("No se encontro un lead con el id indicado.");
        }

        lead.DeletedAtUtc = DateTime.UtcNow;

        await _leadRepository.UpdateAsync(cancellationToken);
    }

    public Task<LeadStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        return _leadRepository.GetStatsAsync(cancellationToken);
    }

    public async Task<string> GenerateSummaryAsync(
        GenerateLeadSummaryCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.FechaDesde.HasValue && command.FechaHasta.HasValue && command.FechaDesde > command.FechaHasta)
        {
            throw new ValidationException("El rango de fechas no es valido: fecha_desde no puede ser mayor que fecha_hasta.");
        }

        var fuente = ParseSourceOrNull(command.Fuente);
        var leads = await _leadRepository.GetForSummaryAsync(
            fuente,
            command.FechaDesde,
            command.FechaHasta,
            cancellationToken);

        if (leads.Count == 0)
        {
            return "No se encontraron leads para generar el resumen con los filtros enviados.";
        }

        return await _leadAiSummaryGenerator.GenerateAsync(
            leads,
            command.Fuente,
            command.FechaDesde,
            command.FechaHasta,
            cancellationToken);
    }

    public Task<PagedResult<LeadResponse>> GetPagedAsync(GetLeadsQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Page < 1)
        {
            throw new ValidationException("El parametro page debe ser mayor o igual a 1.");
        }

        if (query.Limit < 1 || query.Limit > 100)
        {
            throw new ValidationException("El parametro limit debe estar entre 1 y 100.");
        }

        if (query.FechaDesde.HasValue && query.FechaHasta.HasValue && query.FechaDesde > query.FechaHasta)
        {
            throw new ValidationException("El rango de fechas no es valido: fecha_desde no puede ser mayor que fecha_hasta.");
        }

        var fuente = ParseSourceOrNull(query.Fuente);

        return _leadRepository.GetPagedAsync(
            query.Page,
            query.Limit,
            fuente,
            query.FechaDesde,
            query.FechaHasta,
            cancellationToken);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static LeadSource? ParseSourceOrNull(string? fuente)
    {
        if (string.IsNullOrWhiteSpace(fuente))
        {
            return null;
        }

        if (!AllowedSources.TryGetValue(fuente.Trim(), out var source))
        {
            throw new ValidationException("El campo fuente debe ser uno de estos valores: instagram, facebook, landing_page, referido, otro.");
        }

        return source;
    }
}
