using HisabApp.Library.Models;
using Microsoft.EntityFrameworkCore;

namespace HisabApp.Library.Data;

public class HisabDbContext : DbContext
{
    public HisabDbContext(DbContextOptions<HisabDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<ExpenseEntry> Expenses => Set<ExpenseEntry>();
    public DbSet<IncomeEntry> IncomeEntries => Set<IncomeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(64);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Type).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Currency).HasMaxLength(8).IsRequired();
            entity.Property(x => x.CurrencySymbol).HasMaxLength(8).IsRequired();
            entity.Property(x => x.MonthlyExpenseLimit).HasPrecision(18, 2);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<ExpenseEntry>(entity =>
        {
            entity.ToTable("expenses");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(64);
            entity.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Month).HasMaxLength(7).IsRequired();
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Category).HasMaxLength(80).IsRequired();
            entity.Property(x => x.PaymentMethod).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Payee).HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(1000);
            entity.Property(x => x.ReceiptRef).HasMaxLength(100);
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
            entity.HasIndex(x => new { x.TenantId, x.Month });
            entity.HasOne(x => x.Tenant)
                .WithMany(x => x.Expenses)
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IncomeEntry>(entity =>
        {
            entity.ToTable("income_entries");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(64);
            entity.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Month).HasMaxLength(7).IsRequired();
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Category).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Source).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(1000);
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
            entity.HasIndex(x => new { x.TenantId, x.Month });
            entity.HasOne(x => x.Tenant)
                .WithMany(x => x.IncomeEntries)
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
