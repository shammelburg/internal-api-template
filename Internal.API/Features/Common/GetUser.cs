using System.Security.Claims;
using Internal.API.Extensions;
using Internal.API.Interfaces;

namespace Internal.API.Features.Common;

public class GetUser : IEndpoint
{
    public static void Configure(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/", Handle)
            .WithSummary("Get user defaults.")
            .Produces<GetUserDefaultsResponse>();
    }

    private record struct GetUserDefaultsRequest(
        ClaimsPrincipal user
    );

    private record GetUserDefaultsResponse(
        string UserName
    );

    private static async Task<IResult> Handle([AsParameters] GetUserDefaultsRequest req)
    {
        var user = new GetUserDefaultsResponse(
            req.user.GetDomainName()
        );

        return TypedResults.Ok(user);
    }
}