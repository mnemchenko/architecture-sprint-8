using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Handler.WebApi.Features.GetReport;

public class GetReportController : ControllerBase
{
    [HttpGet("reports")]
    public Task<IActionResult> GetReport(CancellationToken cancellationToken)
    {
        var randomData = new
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Data = new[]
            {
                new { Name = "Item1", Value = Random.Shared.Next(1, 100) },
                new { Name = "Item2", Value = Random.Shared.Next(1, 100) },
                new { Name = "Item3", Value = Random.Shared.Next(1, 100) }
            }
        };

        var json = JsonSerializer.Serialize(randomData, new JsonSerializerOptions { WriteIndented = true });

        var fileName = "report.json";
        var fileContent = System.Text.Encoding.UTF8.GetBytes(json);
        var contentType = "application/json";

        return Task.FromResult<IActionResult>(File(fileContent, contentType, fileName));
    }
}