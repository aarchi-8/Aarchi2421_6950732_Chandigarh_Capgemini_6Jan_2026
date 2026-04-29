using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BookStoreFunctions;

public class DailyReport
{
    private readonly ILogger<DailyReport> _logger;
    public DailyReport(ILogger<DailyReport> logger) => _logger = logger;

    [Function("DailyReport")]
    public void Run([TimerTrigger("0 0 22 * * *")] TimerInfo timer)
    {
        _logger.LogInformation("Daily Sales Report generated at {Time}", DateTime.UtcNow);
    }
}