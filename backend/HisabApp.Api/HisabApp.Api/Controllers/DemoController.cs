using HisabApp.Api.Contracts;
using HisabApp.Api.Data;
using HisabApp.Library.Data;
using Microsoft.AspNetCore.Mvc;

namespace HisabApp.Api.Controllers;

[ApiController]
[Route("api/demo")]
public class DemoController : ControllerBase
{
    private readonly HisabDbContext _db;

    public DemoController(HisabDbContext db)
    {
        _db = db;
    }

    [HttpPost("reset")]
    public async Task<ActionResult<DemoSnapshot>> Reset(CancellationToken cancellationToken)
    {
        var snapshot = await DemoDataSeeder.ReseedAsync(_db, cancellationToken);
        return Ok(snapshot);
    }
}
