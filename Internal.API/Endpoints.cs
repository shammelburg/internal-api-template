using Internal.API.Extensions;
using Internal.API.Features.Common;

namespace Internal.API;

public static class Endpoints
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.NotFoundEndpoints();

        var endpoints = app
            .MapGroup("/api")
            .WithOpenApi();

        if (bool.Parse(app.Configuration["UseWindowsAuthentication"]))
        {
            endpoints.RequireAuthorization();
        }

        endpoints.CommonEndpoints();
    }
    
    private static void CommonEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/common")
            .WithTags("Common");

        endpoints
            .MapEndpoint<GetUser>();
    }
    
    private static void NotFoundEndpoints(this IEndpointRouteBuilder app)
    {
        app
            .MapGet("{**catchAll}", () => Results.NotFound("You are lost."))
            .AllowAnonymous()
            .WithTags("Not Found");
    }
}