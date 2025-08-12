namespace Internal.API.Filters;

public class ValidationFilter<T>(IValidator? validator, ILogger<ValidationFilter<T>> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var requestName = typeof(T).FullName;

        if (validator is null)
        {
            logger.LogInformation("{Request}: No validator configured.", requestName);
            return await next(context);
        }

        logger.LogInformation("{Request}: Validating...", requestName);

        var request = context.Arguments.OfType<T>().First();
        var validationResult = await validator.ValidateAsync((IValidationContext)request, context.HttpContext.RequestAborted);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("{Request}: Validation failed.", requestName);
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        logger.LogInformation("{Request}: Validation succeeded.", requestName);
        return await next(context);
    }
}