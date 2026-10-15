namespace HisabApp.Library.Models;

public static class FinanceCatalog
{
    public static readonly string[] TenantTypes = ["personal", "business"];

    public static readonly string[] ExpenseCategories =
    [
        "Grocery & Food",
        "Shopping & Retail",
        "Travel & Fuel",
        "Utilities & Bills",
        "Office & Rent",
        "Team Payroll & Labor",
        "Software & Subscriptions",
        "Marketing & Ads",
        "Healthcare & Medical",
        "Entertainment & Leisure",
        "Maintenance & Repairs",
        "Education & Learning",
        "Other / Miscellaneous"
    ];

    public static readonly string[] IncomeCategories =
    [
        "Fixed Salary",
        "Business Profit / Sales",
        "Client Retainer / Contract",
        "Consulting & Freelance",
        "Rental Income",
        "Investments & Dividends",
        "Side Hustle",
        "Other Income"
    ];

    public static readonly string[] PaymentMethods =
    [
        "Cash",
        "Credit Card",
        "Debit Card",
        "Bank Transfer",
        "Online / Mobile Wallet"
    ];

    public static string MonthOf(DateOnly date) => date.ToString("yyyy-MM");

    public static string? BlankToNull(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }
}
