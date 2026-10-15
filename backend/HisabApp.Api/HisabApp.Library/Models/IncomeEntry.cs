using System.Text.Json.Serialization;

namespace HisabApp.Library.Models;

public class IncomeEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Month { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Tenant? Tenant { get; set; }
}
