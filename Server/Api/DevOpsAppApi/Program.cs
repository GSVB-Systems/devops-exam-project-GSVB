using Microsoft.EntityFrameworkCore;
using Npgsql;
using DevOpsAppRepo;
using DevOpsAppRepo.Interfaces;
using DevOpsAppRepo.Repos;
using DevOpsAppService.Interfaces;
using DevOpsAppService.Services;
using Sieve.Services;

var builder = WebApplication.CreateBuilder(args);

// Read connection string strictly from appsettings.json/appsettings.{Environment}.json
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(conn))
    throw new InvalidOperationException(
        "Connection string 'ConnectionStrings:DefaultConnection' was not found. Configure it in appsettings.json / appsettings.Development.json.");


try
{
    var csb = new NpgsqlConnectionStringBuilder(conn);
    var startupLogger = LoggerFactory.Create(lb => lb.AddConsole()).CreateLogger("Startup");
    startupLogger.LogInformation(
        "Using PostgreSQL connection from appsettings: Host={Host};Port={Port};Database={Database};Username={Username};SslMode={SslMode}",
        csb.Host, csb.Port, csb.Database, csb.Username, csb.SslMode);
}
catch
{
    // Ignore parsing issues; EF/Npgsql will surface a useful error.
}

builder.Services.AddDbContext<DevOpsAppDbContext>(options =>
    options.UseNpgsql(conn, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<ISieveProcessor, SieveProcessor>();
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi();
app.MapControllers();


app.Run();
