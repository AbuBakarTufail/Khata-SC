using System.Text.Json;
using HisabApp.Api.Contracts;
using HisabApp.Library.Data;
using Microsoft.EntityFrameworkCore;

namespace HisabApp.Api.Data;

public static class DemoDataSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task SeedIfEmptyAsync(HisabDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Tenants.AnyAsync(cancellationToken))
        {
            return;
        }

        await ReseedAsync(db, cancellationToken);
    }

    public static async Task<DemoSnapshot> ReseedAsync(HisabDbContext db, CancellationToken cancellationToken = default)
    {
        await db.Expenses.ExecuteDeleteAsync(cancellationToken);
        await db.IncomeEntries.ExecuteDeleteAsync(cancellationToken);
        await db.Tenants.ExecuteDeleteAsync(cancellationToken);

        var snapshot = LoadSnapshot();
        db.Tenants.AddRange(snapshot.Tenants);
        db.Expenses.AddRange(snapshot.Expenses);
        db.IncomeEntries.AddRange(snapshot.Income);
        await db.SaveChangesAsync(cancellationToken);
        return snapshot;
    }

    public static DemoSnapshot LoadSnapshot()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "seed-data.json");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Demo seed file was not found.", path);
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<DemoSnapshot>(json, JsonOptions)
            ?? throw new InvalidOperationException("Demo seed file is empty.");
    }
}
