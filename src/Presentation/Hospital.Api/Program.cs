using Hospital.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel limits for larger request payloads (e.g. video uploads)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100MB
});

// Register API and Infrastructure Services
builder.Services.AddApiServices(builder.Configuration);


var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Hospital API is running!");

// Map all Minimal API endpoints automatically via assembly scanning
app.MapAllEndpoints();

// Seed permissions and default users dynamically at startup
await app.Services.SeedPermissionsAsync();

app.Run();
