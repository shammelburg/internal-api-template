using System.Security.Claims;
using Distrupol.Bank.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine("Using Windows AD");
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        cp => cp
            .WithOrigins(builder.Configuration["ClientAddress"])
            // .AllowAnyOrigin() 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddAuthentication("Windows");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

var endpoints = app
    .MapGroup("/api")
    .WithOpenApi();

if (bool.Parse(app.Configuration["UseWindowsAuthentication"]))
{
    endpoints.RequireAuthorization();
}

endpoints.MapGet("/common",
    (ClaimsPrincipal user) => TypedResults.Ok(new
    {
        username = user.GetDomainName()
    }));


app.Run();