using System.Text.Json;

namespace Internal.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, ILogger<ErrorHandlingMiddleware> logger)
    {
        try
        {
            await _next(httpContext);
        }
        // catch (SqlException ex)
        // {
        //     logger.LogError(ex, "SQL Error");
        //     await HandleSqlExceptionAsync(httpContext, ex);
        // }
        catch (Exception ex)
        {
            logger.LogError(ex, "Server Error");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var result = JsonSerializer.Serialize(
            new
            {
                Type = "General Exception",
                Exception = new { Message = ex.Message }
            }
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;
        return context.Response.WriteAsync(result);
    }

    // private static Task HandleSqlExceptionAsync(HttpContext context, SqlException ex)
    // {
    //     var errorList = new List<Object>();
    //
    //     for (int i = 0; i < ex.Errors.Count; i++)
    //     {
    //         errorList.Add(
    //             new
    //             {
    //                 Message = ex.Errors[i].Message,
    //                 Procedure = ex.Errors[i].Procedure,
    //                 LineNumber = ex.Errors[i].LineNumber,
    //                 Source = ex.Errors[i].Source,
    //                 Server = ex.Errors[i].Server
    //             }
    //         );
    //     }
    //
    //     var result = JsonSerializer.Serialize(
    //         new { Type = "SQL Exception", Exceptions = errorList }
    //     );
    //
    //     context.Response.ContentType = "application/json";
    //     context.Response.StatusCode = 500;
    //     return context.Response.WriteAsync(result);
    // }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}