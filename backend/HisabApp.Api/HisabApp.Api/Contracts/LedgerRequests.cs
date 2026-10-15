namespace HisabApp.Api.Contracts;

public class SaveTenantRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "personal";
    public string Currency { get; set; } = "USD";
    public string CurrencySymbol { get; set; } = "$";
    public decimal MonthlyExpenseLimit { get; set; }
    public int FiscalMonthStart { get; set; } = 1;
    public string? Description { get; set; }
}

public class SaveExpenseRequest
{
    public string TenantId { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Payee { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ReceiptRef { get; set; }
}

public class SaveIncomeRequest
{
    public string TenantId { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
    public string? Notes { get; set; }
}

public class DemoSnapshot
{
    public List<HisabApp.Library.Models.Tenant> Tenants { get; set; } = [];
    public List<HisabApp.Library.Models.ExpenseEntry> Expenses { get; set; } = [];
    public List<HisabApp.Library.Models.IncomeEntry> Income { get; set; } = [];
}
