using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OneMillionCopy.Leads.Application.Abstractions.Persistence;
using OneMillionCopy.Leads.Application.Leads.Services;
using OneMillionCopy.Leads.Infrastructure.AI;
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

        services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.SectionName));
        services.AddHttpClient<OpenAiLeadSummaryGenerator>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<OpenAiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        services.AddScoped<MockLeadSummaryGenerator>();
        services.AddScoped<ILeadAiSummaryGenerator>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<OpenAiOptions>>().Value;
            return string.IsNullOrWhiteSpace(options.ApiKey)
                ? serviceProvider.GetRequiredService<MockLeadSummaryGenerator>()
                : serviceProvider.GetRequiredService<OpenAiLeadSummaryGenerator>();
        });

        services.AddScoped<ApplicationDbInitializer>();
        services.AddScoped<ILeadRepository, LeadRepository>();

        return services;
    }
}
