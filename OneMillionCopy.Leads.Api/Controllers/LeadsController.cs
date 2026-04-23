using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using OneMillionCopy.Leads.Api.Contracts.Leads;
using OneMillionCopy.Leads.Application.Common.Exceptions;
using OneMillionCopy.Leads.Application.Leads.Commands.CreateLead;
using OneMillionCopy.Leads.Application.Leads.Commands.UpdateLead;
using OneMillionCopy.Leads.Application.Leads.Queries.GetLeads;
using OneMillionCopy.Leads.Application.Leads.Services;

namespace OneMillionCopy.Leads.Api.Controllers;

[ApiController]
[Route("leads")]
public sealed class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;

    public LeadsController(ILeadService leadService)
    {
        _leadService = leadService;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _leadService.GetByIdAsync(id, cancellationToken);

        return Ok(response);
    }

    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var response = await _leadService.GetStatsAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetLeadsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetLeadsQuery(
            request.Page,
            request.Limit,
            request.Fuente,
            request.FechaDesde,
            request.FechaHasta);

        var response = await _leadService.GetPagedAsync(query, cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateLeadRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLeadCommand(
            id,
            request.Nombre.HasValue,
            ReadOptionalString(request.Nombre),
            request.Email.HasValue,
            ReadOptionalString(request.Email),
            request.Telefono.HasValue,
            ReadOptionalString(request.Telefono),
            request.Fuente.HasValue,
            ReadOptionalString(request.Fuente),
            request.ProductoInteres.HasValue,
            ReadOptionalString(request.ProductoInteres),
            request.Presupuesto.HasValue,
            ReadOptionalDecimal(request.Presupuesto));

        var response = await _leadService.UpdateAsync(command, cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _leadService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateLeadRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLeadCommand(
            request.Nombre,
            request.Email,
            request.Telefono,
            request.Fuente,
            request.ProductoInteres,
            request.Presupuesto);

        var response = await _leadService.CreateAsync(command, cancellationToken);

        return Created($"/leads/{response.Id}", response);
    }

    private static string? ReadOptionalString(JsonElement? element)
    {
        if (!element.HasValue || element.Value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return element.Value.ValueKind == JsonValueKind.String
            ? element.Value.GetString()
            : element.Value.ToString();
    }

    private static decimal? ReadOptionalDecimal(JsonElement? element)
    {
        if (!element.HasValue || element.Value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (element.Value.ValueKind != JsonValueKind.Number || !element.Value.TryGetDecimal(out var value))
        {
            throw new ValidationException("El campo presupuesto debe ser numerico.");
        }

        return value;
    }
}
