namespace Internal.API;

public static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
    }

    public static void AddCORS(this WebApplicationBuilder builder)
    {
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
    }
}