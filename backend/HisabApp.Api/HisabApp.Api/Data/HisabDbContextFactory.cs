using HisabApp.Library.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HisabApp.Api.Data;

public class HisabDbContextFactory : IDesignTimeDbContextFactory<HisabDbContext>
{
    public HisabDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=hisabdb;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<HisabDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(typeof(HisabDbContext).Assembly.GetName().Name))
            .Options;

        return new HisabDbContext(options);
    }
}
