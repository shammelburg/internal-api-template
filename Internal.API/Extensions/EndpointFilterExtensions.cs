using Internal.API.Filters;

namespace Internal.API.Extensions;

public static class EndpointFilterExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<T>(this RouteHandlerBuilder builder)
    {
        return builder
            .AddEndpointFilter<ValidationFilter<T>>()
            .ProducesValidationProblem();
    }
}