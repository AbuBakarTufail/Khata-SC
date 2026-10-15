using System.Text.Json.Serialization;

namespace HisabApp.Library.Models;

public class Tenant
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "personal";
    public string Currency { get; set; } = "USD";
    public string CurrencySymbol { get; set; } = "$";
    public decimal MonthlyExpenseLimit { get; set; }
    public int FiscalMonthStart { get; set; } = 1;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<ExpenseEntry> Expenses { get; set; } = new List<ExpenseEntry>();

    [JsonIgnore]
    public ICollection<IncomeEntry> IncomeEntries { get; set; } = new List<IncomeEntry>();
}
