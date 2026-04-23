using Microsoft.EntityFrameworkCore;
using OneMillionCopy.Leads.Domain.Entities;
using OneMillionCopy.Leads.Domain.Enums;

namespace OneMillionCopy.Leads.Infrastructure.Persistence;

public sealed class ApplicationDbInitializer
{
    private readonly ApplicationDbContext _dbContext;

    public ApplicationDbInitializer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await SeedAsync(cancellationToken);
    }

    private async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.Leads.AnyAsync(cancellationToken))
        {
            return;
        }

        var utcNow = DateTime.UtcNow;

        var leads = new[]
        {
            new Lead { Id = Guid.Parse("a1b2c3d4-e5f6-4711-8001-111111111111"), Nombre = "Laura Mendoza", Email = "laura.mendoza@example.com", Telefono = "+573001112233", Fuente = LeadSource.Instagram, ProductoInteres = "Curso de copywriting", Presupuesto = 120m, CreatedAtUtc = utcNow.AddDays(-1), UpdatedAtUtc = utcNow.AddDays(-1) },
            new Lead { Id = Guid.Parse("a1b2c3d4-e5f6-4711-8002-222222222222"), Nombre = "Carlos Perez", Email = "carlos.perez@example.com", Telefono = "+573002223344", Fuente = LeadSource.Facebook, ProductoInteres = "Mentoria premium", Presupuesto = 300m, CreatedAtUtc = utcNow.AddDays(-2), UpdatedAtUtc = utcNow.AddDays(-2) },
            new Lead { Id = Guid.Parse("a1b2c3d4-e5f6-4711-8003-333333333333"), Nombre = "Ana Torres", Email = "ana.torres@example.com", Telefono = "+573003334455", Fuente = LeadSource.LandingPage, ProductoInteres = "Plantillas de email", Presupuesto = 80m, CreatedAtUtc = utcNow.AddDays(-3), UpdatedAtUtc = utcNow.AddDays(-3) },
            new Lead { Id = Guid.Parse("a1b2c3d4-e5f6-4711-8004-444444444444"), Nombre = "Mateo Rojas", Email = "mateo.rojas@example.com", Telefono = null, Fuente = LeadSource.Referido, ProductoInteres = "Bootcamp ventas", Presupuesto = 450m, CreatedAtUtc = utcNow.AddDays(-4), UpdatedAtUtc = utcNow.AddDays(-4) },
            new Lead { Id = Guid.Parse("a1b2c3d4-e5f6-4711-8005-555555555555"), Nombre = "Sofia Vargas", Email = "sofia.vargas@example.com", Telefono = "+573004445566", Fuente = LeadSource.Otro, ProductoInteres = "Workshop embudos", Presupuesto = 150m, CreatedAtUtc = utcNow.AddDays(-5), UpdatedAtUtc = utcNow.AddDays(-5) },
            new Lead { Id = Guid.Parse("a1b2c3d4-e5f6-4711-8006-666666666666"), Nombre = "Daniel Castro", Email = "daniel.castro@example.com", Telefono = "+573005556677", Fuente = LeadSource.Instagram, ProductoInteres = "Consultoria", Presupuesto = 600m, CreatedAtUtc = utcNow.AddDays(-6), UpdatedAtUtc = utcNow.AddDays(-6) },
            new Lead { Id = Guid.Parse("a1b2c3d4-e5f6-4711-8007-777777777777"), Nombre = "Valentina Ruiz", Email = "valentina.ruiz@example.com", Telefono = null, Fuente = LeadSource.Facebook, ProductoInteres = "Curso anuncios", Presupuesto = 200m, CreatedAtUtc = utcNow.AddDays(-7), UpdatedAtUtc = utcNow.AddDays(-7) },
            new Lead { Id = Guid.Parse("a1b2c3d4-e5f6-4711-8008-888888888888"), Nombre = "Julian Lopez", Email = "julian.lopez@example.com", Telefono = "+573006667788", Fuente = LeadSource.LandingPage, ProductoInteres = "Comunidad privada", Presupuesto = null, CreatedAtUtc = utcNow.AddDays(-8), UpdatedAtUtc = utcNow.AddDays(-8) },
            new Lead { Id = Guid.Parse("a1b2c3d4-e5f6-4711-8009-999999999999"), Nombre = "Camila Herrera", Email = "camila.herrera@example.com", Telefono = "+573007778899", Fuente = LeadSource.Referido, ProductoInteres = "Masterclass", Presupuesto = 95m, CreatedAtUtc = utcNow.AddDays(-9), UpdatedAtUtc = utcNow.AddDays(-9) },
            new Lead { Id = Guid.Parse("a1b2c3d4-e5f6-4711-8010-101010101010"), Nombre = "Nicolas Gomez", Email = "nicolas.gomez@example.com", Telefono = "+573008889900", Fuente = LeadSource.Otro, ProductoInteres = "Auditoria funnel", Presupuesto = 750m, CreatedAtUtc = utcNow.AddDays(-10), UpdatedAtUtc = utcNow.AddDays(-10) }
        };

        await _dbContext.Leads.AddRangeAsync(leads, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
