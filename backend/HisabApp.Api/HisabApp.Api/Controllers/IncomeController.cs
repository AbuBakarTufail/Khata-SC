using HisabApp.Api.Contracts;
using HisabApp.Library.Data;
using HisabApp.Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HisabApp.Api.Controllers;

[ApiController]
[Route("api/income")]
public class IncomeController : ControllerBase
{
    private readonly HisabDbContext _db;

    public IncomeController(HisabDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<IncomeEntry>>> GetAll(
        [FromQuery] string? tenantId,
        [FromQuery] string? month,
        CancellationToken cancellationToken)
    {
        var query = _db.IncomeEntries.AsQueryable();
        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            query = query.Where(i => i.TenantId == tenantId);
        }

        if (!string.IsNullOrWhiteSpace(month))
        {
            query = query.Where(i => i.Month == month);
        }

        var income = await query
            .OrderByDescending(i => i.Date)
            .ThenByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
        return Ok(income);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IncomeEntry>> GetById(string id, CancellationToken cancellationToken)
    {
        var entry = await _db.IncomeEntries.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        return entry is null ? NotFound(new { message = "Income entry was not found." }) : Ok(entry);
    }

    [HttpPost]
    public async Task<ActionResult<IncomeEntry>> Create(SaveIncomeRequest request, CancellationToken cancellationToken)
    {
        var error = await ValidateAsync(request, cancellationToken);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }

        var entry = new IncomeEntry
        {
            TenantId = request.TenantId.Trim(),
            Date = request.Date,
            Month = FinanceCatalog.MonthOf(request.Date),
            Amount = request.Amount,
            Category = request.Category.Trim(),
            Source = request.Source.Trim(),
            IsRecurring = request.IsRecurring,
            Notes = FinanceCatalog.BlankToNull(request.Notes),
            CreatedAt = DateTime.UtcNow
        };

        _db.IncomeEntries.Add(entry);
        await _db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<IncomeEntry>> Update(string id, SaveIncomeRequest request, CancellationToken cancellationToken)
    {
        var entry = await _db.IncomeEntries.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (entry is null)
        {
            return NotFound(new { message = "Income entry was not found." });
        }

        var error = await ValidateAsync(request, cancellationToken);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }

        entry.TenantId = request.TenantId.Trim();
        entry.Date = request.Date;
        entry.Month = FinanceCatalog.MonthOf(request.Date);
        entry.Amount = request.Amount;
        entry.Category = request.Category.Trim();
        entry.Source = request.Source.Trim();
        entry.IsRecurring = request.IsRecurring;
        entry.Notes = FinanceCatalog.BlankToNull(request.Notes);

        await _db.SaveChangesAsync(cancellationToken);
        return Ok(entry);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var deleted = await _db.IncomeEntries.Where(i => i.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deleted == 0 ? NotFound(new { message = "Income entry was not found." }) : NoContent();
    }

    private async Task<string?> ValidateAsync(SaveIncomeRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenantId))
        {
            return "Workspace is required.";
        }

        if (request.Date == default)
        {
            return "Income date is required.";
        }

        if (request.Amount <= 0)
        {
            return "Amount must be greater than zero.";
        }

        if (string.IsNullOrWhiteSpace(request.Source))
        {
            return "Revenue source is required.";
        }

        if (!FinanceCatalog.IncomeCategories.Contains(request.Category?.Trim()))
        {
            return "Income category is not recognized.";
        }

        var tenantExists = await _db.Tenants.AnyAsync(t => t.Id == request.TenantId.Trim(), cancellationToken);
        return tenantExists ? null : "Workspace was not found.";
    }
}
