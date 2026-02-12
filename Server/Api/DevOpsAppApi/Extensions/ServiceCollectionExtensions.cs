using System.Net.Http.Headers;
using System.Text;
using DevOpsAppRepo;
using DevOpsAppRepo.Interfaces;
using DevOpsAppRepo.Repos;
using DevOpsAppService.Auth;
using DevOpsAppService.EggApi;
using DevOpsAppService.Interfaces;
using DevOpsAppService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Sieve.Services;

namespace DevOpsAppApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool addDefaultDbContext = true,
        Action<IServiceCollection>? configureOverrides = null
    )
    {
        // Only require the connection string when we intend to register the default DbContext
        if (addDefaultDbContext)
        {
            // Read connection string strictly from appsettings.json/appsettings.{Environment}.json
            var conn = configuration.GetConnectionString("DefaultConnection");

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
            services.AddDbContext<DevOpsAppDbContext>(options =>
                options.UseNpgsql(conn, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));
            
        }

        var jwtSection = configuration.GetSection("Jwt");
        var jwtOptions = jwtSection.Get<JwtOptions>() ?? throw new InvalidOperationException("Missing Jwt configuration.");
        if (string.IsNullOrWhiteSpace(jwtOptions.Secret))
            throw new InvalidOperationException("Jwt:Secret must be configured (appsettings.Development.json or environment variables).");

        var secretByteLength = Encoding.UTF8.GetByteCount(jwtOptions.Secret);
        if (secretByteLength < 32)
            throw new InvalidOperationException("Jwt:Secret must be at least 32 bytes (256 bits).");



        services.AddSingleton(jwtOptions);
        services.AddSingleton<JwtTokenService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddControllers();
        services.AddOpenApiDocument();
        services.AddScoped<IUserEggSnapshotRepository, UserEggSnapshotRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEggAccountService, EggAccountService>();
        services.AddScoped<IEggSnapshotService, EggSnapshotService>();
        services.AddScoped<PasswordService>();
        services.AddScoped<ISieveProcessor, SieveProcessor>();
        services.Configure<EggApiOptions>(configuration.GetSection(EggApiOptions.SectionName));
        services.AddHttpClient<IEggApiClient, EggApiClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<EggApiOptions>>().Value;
            if (string.IsNullOrWhiteSpace(options.AuxbrainBaseUrl))
                throw new InvalidOperationException("EggApi:AuxbrainBaseUrl must be configured in appsettings.json.");

            client.BaseAddress = new Uri(options.AuxbrainBaseUrl.TrimEnd('/'));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
        });

       


        services.AddSingleton(jwtOptions);
        services.AddSingleton<JwtTokenService>();

        

        services.AddAuthorization();
        

        // Allow tests (or other callers) to override registrations, e.g., swap DbContext with a test container
        configureOverrides?.Invoke(services);

        return services;
    }
}