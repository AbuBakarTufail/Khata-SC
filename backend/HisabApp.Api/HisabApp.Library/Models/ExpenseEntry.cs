using System.Text.Json.Serialization;

namespace HisabApp.Library.Models;

public class ExpenseEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Month { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Payee { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ReceiptRef { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Tenant? Tenant { get; set; }
}
