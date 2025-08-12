using Internal.API.Interfaces;

namespace Internal.API.Extensions;

public static class EndpointRouteBuilderExtension
{
    public static IEndpointRouteBuilder MapEndpoint<T>(this IEndpointRouteBuilder app)
        where T : IEndpoint
    {
        T.Configure(app);
        return app;
    }
}