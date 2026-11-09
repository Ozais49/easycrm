using Cem.Crm.Endpoints.Companies;
using Cem.Crm.Endpoints.Contacts;
using Microsoft.AspNetCore.Routing;

namespace Cem.Crm.Endpoints;

public static class CrmEndpoints
{
    /// <summary>The tenant-scoped endpoints; map them onto the <c>/api</c> group that requires a tenant.</summary>
    public static IEndpointRouteBuilder MapCrmApi(this IEndpointRouteBuilder api) =>
        api.MapCompanies().MapContacts();
}
