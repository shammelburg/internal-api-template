using System.Security.Claims;
using Distrupol.Bank.API.Extensions;
using Distrupol.Bank.API.Models.QueryParams;

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
            .WithOrigins(builder.Configuration["ClientAddress"]) // Remove for Add-in
            // .AllowAnyOrigin()  // Remove for Add-in
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()); // Remove for Add-in
});

builder.Services.AddAuthentication("Windows");

var app = builder.Build();

// Force FileWatcherService to initialize
// app.Services.GetRequiredService<FileWatcherService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

// Map the SignalR hub endpoint
// app.MapHub<FolderHub>("/folderhub");

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
        username = user.GetDomainName(),
        sharedDrives = builder.Configuration.GetSection("SharedDrives").Get<string[]>(),
        DestinationDrive = builder.Configuration["DestinationDrive"]
    }));

endpoints.MapPost("/shared-drive",
    (SharedDriveQueryParams qp, ClaimsPrincipal user) =>
    {
        if (!Directory.Exists(qp.SharedDrive))
        {
            return Results.BadRequest("The directory path does not exist");
        }

        var files = Directory.GetFiles(qp.SharedDrive).Where(f =>
        {
            var fileName = Path.GetFileNameWithoutExtension(f);
            return !fileName.StartsWith($".") && !string.IsNullOrEmpty(fileName);
        });

        var filesWithInfo =
            files.Select(file => new { file = Path.GetFileName(file), info = File.GetCreationTime(file) });


        return TypedResults.Ok(new { files = filesWithInfo });
    });

endpoints.MapPost("/destination-drive",
    (DestinationDriveQueryParams qp, ClaimsPrincipal user) =>
    {
        var year = DateTime.Today.Year.ToString();
        var month = DateTime.Today.Month.ToString().PadLeft(2, '0');
        var destination = Path.Combine(qp.DestinationDrive, year, month);

        if (!Directory.Exists(destination))
        {
            Directory.CreateDirectory(destination);
        }

        var files = Directory.GetFiles(qp.SharedDrive).Where(f =>
        {
            var fileName = Path.GetFileNameWithoutExtension(f);
            return !fileName.StartsWith($".") && !string.IsNullOrEmpty(fileName);
        });

        if (qp.Uppercase)
        {
            qp.BatchNumber = qp.BatchNumber.ToUpper();
        }

        var fileCount = 0;
        var list = new List<string>();
        foreach (var file in files)
        {
            fileCount = fileCount + 1;

            var fileName = Path.GetFileNameWithoutExtension(file);
            var fileNameBatchNumber = qp.BatchNumber + $"-{fileCount}";

            if (files.Count() == 1)
            {
                fileNameBatchNumber = qp.BatchNumber;
            }

            var dest = file.Replace(qp.SharedDrive, destination).Replace(fileName, fileNameBatchNumber);

            File.Move(file, dest);
            list.Add(dest.Replace(qp.DestinationDrive, ""));
        }

        return TypedResults.Ok(new { list = list });
    });

app.Run();