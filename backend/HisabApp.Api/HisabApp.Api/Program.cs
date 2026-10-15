using HisabApp.Api.Data;
using HisabApp.Library.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<HisabDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsAssembly(typeof(HisabDbContext).Assembly.GetName().Name)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("HisabDatabase");
    var db = scope.ServiceProvider.GetRequiredService<HisabDbContext>();
    try
    {
        await db.Database.MigrateAsync();
        await DemoDataSeeder.SeedIfEmptyAsync(db);
    }
    catch (Exception ex)
    {
        logger.LogCritical(
            ex,
            "PostgreSQL is not available. Start the server and confirm ConnectionStrings:DefaultConnection in appsettings.json (database hisabdb on localhost:5432).");
        throw;
    }
}

app.Run();
