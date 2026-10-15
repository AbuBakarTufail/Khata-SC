using HisabApp.Api.Contracts;
using HisabApp.Library.Data;
using HisabApp.Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HisabApp.Api.Controllers;

[ApiController]
[Route("api/tenants")]
public class TenantsController : ControllerBase
{
    private readonly HisabDbContext _db;

    public TenantsController(HisabDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Tenant>>> GetAll(CancellationToken cancellationToken)
    {
        var tenants = await _db.Tenants
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
        return Ok(tenants);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Tenant>> GetById(string id, CancellationToken cancellationToken)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return tenant is null ? NotFound(new { message = "Workspace was not found." }) : Ok(tenant);
    }

    [HttpPost]
    public async Task<ActionResult<Tenant>> Create(SaveTenantRequest request, CancellationToken cancellationToken)
    {
        var error = Validate(request);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }

        var tenant = new Tenant
        {
            Name = request.Name.Trim(),
            Type = request.Type.Trim().ToLowerInvariant(),
            Currency = request.Currency.Trim().ToUpperInvariant(),
            CurrencySymbol = request.CurrencySymbol.Trim(),
            MonthlyExpenseLimit = request.MonthlyExpenseLimit,
            FiscalMonthStart = request.FiscalMonthStart,
            Description = FinanceCatalog.BlankToNull(request.Description),
            CreatedAt = DateTime.UtcNow
        };

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, tenant);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Tenant>> Update(string id, SaveTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tenant is null)
        {
            return NotFound(new { message = "Workspace was not found." });
        }

        var error = Validate(request);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }

        tenant.Name = request.Name.Trim();
        tenant.Type = request.Type.Trim().ToLowerInvariant();
        tenant.Currency = request.Currency.Trim().ToUpperInvariant();
        tenant.CurrencySymbol = request.CurrencySymbol.Trim();
        tenant.MonthlyExpenseLimit = request.MonthlyExpenseLimit;
        tenant.FiscalMonthStart = request.FiscalMonthStart;
        tenant.Description = FinanceCatalog.BlankToNull(request.Description);

        await _db.SaveChangesAsync(cancellationToken);
        return Ok(tenant);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var count = await _db.Tenants.CountAsync(cancellationToken);
        if (count <= 1)
        {
            return Conflict(new { message = "You cannot delete the only remaining workspace." });
        }

        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tenant is null)
        {
            return NotFound(new { message = "Workspace was not found." });
        }

        _db.Tenants.Remove(tenant);
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static string? Validate(SaveTenantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return "Workspace name is required.";
        }

        if (!FinanceCatalog.TenantTypes.Contains(request.Type?.Trim().ToLowerInvariant()))
        {
            return "Workspace type must be personal or business.";
        }

        if (string.IsNullOrWhiteSpace(request.Currency) || string.IsNullOrWhiteSpace(request.CurrencySymbol))
        {
            return "Currency and currency symbol are required.";
        }

        if (request.MonthlyExpenseLimit < 0)
        {
            return "Monthly expense limit cannot be negative.";
        }

        if (request.FiscalMonthStart is < 1 or > 28)
        {
            return "Fiscal month start must be a day between 1 and 28.";
        }

        return null;
    }
}
