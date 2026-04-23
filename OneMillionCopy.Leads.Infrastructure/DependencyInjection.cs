using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneMillionCopy.Leads.Application.Abstractions.Persistence;
using OneMillionCopy.Leads.Infrastructure.Persistence;
using OneMillionCopy.Leads.Infrastructure.Persistence.Repositories;

namespace OneMillionCopy.Leads.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("No se encontro la cadena de conexion 'SqlServer'.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ILeadRepository, LeadRepository>();

        return services;
    }
}
