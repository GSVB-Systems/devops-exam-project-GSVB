using DevOpsAppApi.Extensions;
using DevOpsAppRepo;
using DevOpsAppService.Interfaces;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace Test;

public class Startup
{
    
    public static void ConfigureServices(IServiceCollection services)
    {
        Env.Load();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddApiServices(configuration, addDefaultDbContext: false, configureOverrides =>
        {
            configureOverrides.RemoveAll(typeof(DevOpsAppDbContext));
            configureOverrides.RemoveAll(typeof(IEggApiClient));

            configureOverrides.AddScoped<DevOpsAppDbContext>(_ =>
            {
                var postgreSqlContainer = new PostgreSqlBuilder().Build();
                postgreSqlContainer.StartAsync().GetAwaiter().GetResult();
                var connectionString = postgreSqlContainer.GetConnectionString();
                var options = new DbContextOptionsBuilder<DevOpsAppDbContext>()
                    .UseNpgsql(connectionString)
                    .Options;

                var ctx = new DevOpsAppDbContext(options);
                ctx.Database.EnsureCreated();
                return ctx;
            });

            configureOverrides.AddSingleton<FakeEggApiClient>();
            configureOverrides.AddSingleton<IEggApiClient>(sp => sp.GetRequiredService<FakeEggApiClient>());
        });
    }
}