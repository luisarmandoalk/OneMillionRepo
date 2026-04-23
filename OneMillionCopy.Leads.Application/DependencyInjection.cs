using Microsoft.Extensions.DependencyInjection;
using OneMillionCopy.Leads.Application.Leads.Services;

namespace OneMillionCopy.Leads.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILeadService, LeadService>();

        return services;
    }
}
