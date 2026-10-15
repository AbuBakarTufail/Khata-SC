using HisabApp.Api.Contracts;
using HisabApp.Library.Data;
using HisabApp.Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HisabApp.Api.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpensesController : ControllerBase
{
    private readonly HisabDbContext _db;

    public ExpensesController(HisabDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<ExpenseEntry>>> GetAll(
        [FromQuery] string? tenantId,
        [FromQuery] string? month,
        CancellationToken cancellationToken)
    {
        var query = _db.Expenses.AsQueryable();
        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            query = query.Where(e => e.TenantId == tenantId);
        }

        if (!string.IsNullOrWhiteSpace(month))
        {
            query = query.Where(e => e.Month == month);
        }

        var expenses = await query
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
        return Ok(expenses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseEntry>> GetById(string id, CancellationToken cancellationToken)
    {
        var expense = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        return expense is null ? NotFound(new { message = "Expense was not found." }) : Ok(expense);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseEntry>> Create(SaveExpenseRequest request, CancellationToken cancellationToken)
    {
        var error = await ValidateAsync(request, cancellationToken);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }

        var expense = new ExpenseEntry
        {
            TenantId = request.TenantId.Trim(),
            Date = request.Date,
            Month = FinanceCatalog.MonthOf(request.Date),
            Amount = request.Amount,
            Category = request.Category.Trim(),
            PaymentMethod = request.PaymentMethod.Trim(),
            Payee = FinanceCatalog.BlankToNull(request.Payee),
            Description = request.Description.Trim(),
            Notes = FinanceCatalog.BlankToNull(request.Notes),
            ReceiptRef = FinanceCatalog.BlankToNull(request.ReceiptRef),
            CreatedAt = DateTime.UtcNow
        };

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ExpenseEntry>> Update(string id, SaveExpenseRequest request, CancellationToken cancellationToken)
    {
        var expense = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (expense is null)
        {
            return NotFound(new { message = "Expense was not found." });
        }

        var error = await ValidateAsync(request, cancellationToken);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }

        expense.TenantId = request.TenantId.Trim();
        expense.Date = request.Date;
        expense.Month = FinanceCatalog.MonthOf(request.Date);
        expense.Amount = request.Amount;
        expense.Category = request.Category.Trim();
        expense.PaymentMethod = request.PaymentMethod.Trim();
        expense.Payee = FinanceCatalog.BlankToNull(request.Payee);
        expense.Description = request.Description.Trim();
        expense.Notes = FinanceCatalog.BlankToNull(request.Notes);
        expense.ReceiptRef = FinanceCatalog.BlankToNull(request.ReceiptRef);

        await _db.SaveChangesAsync(cancellationToken);
        return Ok(expense);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var deleted = await _db.Expenses.Where(e => e.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deleted == 0 ? NotFound(new { message = "Expense was not found." }) : NoContent();
    }

    private async Task<string?> ValidateAsync(SaveExpenseRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenantId))
        {
            return "Workspace is required.";
        }

        if (request.Date == default)
        {
            return "Expense date is required.";
        }

        if (request.Amount <= 0)
        {
            return "Amount must be greater than zero.";
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return "Description is required.";
        }

        if (!FinanceCatalog.ExpenseCategories.Contains(request.Category?.Trim()))
        {
            return "Expense category is not recognized.";
        }

        if (!FinanceCatalog.PaymentMethods.Contains(request.PaymentMethod?.Trim()))
        {
            return "Payment method is not recognized.";
        }

        var tenantExists = await _db.Tenants.AnyAsync(t => t.Id == request.TenantId.Trim(), cancellationToken);
        return tenantExists ? null : "Workspace was not found.";
    }
}
