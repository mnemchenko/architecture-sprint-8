using Microsoft.AspNetCore.Mvc;

namespace Handler.WebApi.Features.GetReport;

public class GetReportController : ControllerBase
{
    [HttpGet("reports")]
    public async Task<IActionResult> GetReport(CancellationToken cancellationToken)
    {
        return Ok();
    }
}