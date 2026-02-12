using System;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using DevOpsAppRepo.Entities;

namespace DevOpsAppRepo;

public class DevOpsAppDbContext : DbContext
{
    public DevOpsAppDbContext(DbContextOptions<DevOpsAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserEggSnapshot> UserEggSnapshots { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        Env.Load();

        
        var conn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                   ?? Environment.GetEnvironmentVariable("CONNECTION_STRING")
                   ?? Environment.GetEnvironmentVariable("DATABASE_URL");

        if (string.IsNullOrWhiteSpace(conn))
            throw new InvalidOperationException("No database connection string found in environment (ConnectionStrings__DefaultConnection, CONNECTION_STRING or DATABASE_URL).");

        if (conn.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            conn = BuildConnectionStringFromDatabaseUrl(conn);

        optionsBuilder.UseNpgsql(conn, b => b.EnableRetryOnFailure());
    }

    private static string BuildConnectionStringFromDatabaseUrl(string databaseUrl)
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Username = userInfo.Length > 0 ? userInfo[0] : string.Empty,
            Password = userInfo.Length > 1 ? userInfo[1] : string.Empty,
            Database = uri.AbsolutePath.TrimStart('/'),
            SslMode = SslMode.Prefer,
            TrustServerCertificate = true
        };
        return builder.ToString();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(eb =>
        {
            eb.HasKey(u => u.UserId);
            eb.Property(u => u.UserId).IsRequired();
            eb.Property(u => u.Username).IsRequired();
            eb.Property(u => u.Email).IsRequired();
        });

        modelBuilder.Entity<UserEggSnapshot>(eb =>
        {
            eb.HasKey(s => s.Id);
            eb.Property(s => s.Id).IsRequired();
            eb.Property(s => s.UserId).IsRequired();
            eb.Property(s => s.EiUserId).IsRequired();
            eb.Property(s => s.Status).IsRequired();
            eb.Property(s => s.LastFetchedUtc).IsRequired();
            eb.Property(s => s.RawJson).HasColumnType("jsonb").IsRequired();
            eb.HasIndex(s => new { s.UserId, s.EiUserId }).IsUnique();
            eb.HasOne<User>().WithMany().HasForeignKey(s => s.UserId);
        });

        base.OnModelCreating(modelBuilder);
    }
}