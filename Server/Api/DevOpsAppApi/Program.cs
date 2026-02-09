using Microsoft.EntityFrameworkCore;
using Npgsql;
using DevOpsAppRepo;
using DevOpsAppRepo.Interfaces;
using DevOpsAppRepo.Repos;
using DevOpsAppService.Interfaces;
using DevOpsAppService.Services;
using Sieve.Services;
using DevOpsAppService.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DevOpsAppService.EggApi;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

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

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();
builder.Services.AddScoped<IUserEggSnapshotRepository, UserEggSnapshotRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEggSnapshotService, EggSnapshotService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<ISieveProcessor, SieveProcessor>();
builder.Services.Configure<EggApiOptions>(builder.Configuration.GetSection(EggApiOptions.SectionName));
builder.Services.AddHttpClient<IEggApiClient, EggApiClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<EggApiOptions>>().Value;
    if (string.IsNullOrWhiteSpace(options.AuxbrainBaseUrl))
        throw new InvalidOperationException("EggApi:AuxbrainBaseUrl must be configured in appsettings.json.");

    client.BaseAddress = new Uri(options.AuxbrainBaseUrl.TrimEnd('/'));
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
});

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<JwtOptions>() ?? throw new InvalidOperationException("Missing Jwt configuration.");
if (string.IsNullOrWhiteSpace(jwtOptions.Secret))
    throw new InvalidOperationException("Jwt:Secret must be configured (appsettings.Development.json or environment variables).");

var secretByteLength = Encoding.UTF8.GetByteCount(jwtOptions.Secret);
if (secretByteLength < 32)
    throw new InvalidOperationException("Jwt:Secret must be at least 32 bytes (256 bits).");



builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton<JwtTokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
