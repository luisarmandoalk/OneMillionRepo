using OneMillionCopy.Leads.Application.Common.Exceptions;
using OneMillionCopy.Leads.Domain.Enums;

namespace OneMillionCopy.Leads.Application.Leads;

public static class LeadSourceMapper
{
    public static string ToApiValue(LeadSource source) =>
        source switch
        {
            LeadSource.Instagram => "instagram",
            LeadSource.Facebook => "facebook",
            LeadSource.LandingPage => "landing_page",
            LeadSource.Referido => "referido",
            LeadSource.Otro => "otro",
            _ => throw new ValidationException("La fuente indicada no es valida.")
        };
}
