using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("sharedsettings.json", false, true);
}
else
{
    builder.Configuration.AddJsonFile("/run/secrets/shared_settings", false);
}

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddDbContextPool<MalContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "./src/static")),
    RequestPath = "/api/static"
});

app.MapControllers();

app.Run();
