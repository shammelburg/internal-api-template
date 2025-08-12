using Internal.API;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();
builder.AddCORS();

builder.Services.AddAuthentication("Windows");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => { options.RouteTemplate = "openapi/{documentName}.json"; });
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Internal API")
            .WithTheme(ScalarTheme.Moon);
    });
}

var endpoints = app
    .MapGroup("/api")
    .WithOpenApi();

if (bool.Parse(app.Configuration["UseWindowsAuthentication"]))
{
    endpoints.RequireAuthorization();
}

app.UseCors("CorsPolicy");

app.UseAuthorization();
app.UseAuthentication();

app.MapApiEndpoints();

app.Run();