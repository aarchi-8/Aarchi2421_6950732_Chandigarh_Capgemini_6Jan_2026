using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BookStoreFunctions;

public class ProcessOrder
{
    private readonly ILogger<ProcessOrder> _logger;
    public ProcessOrder(ILogger<ProcessOrder> logger) => _logger = logger;

    [Function("ProcessOrder")]
    public void Run([QueueTrigger("orders-queue", Connection = "AzureWebJobsStorage")] string order)
    {
        _logger.LogInformation("Processing Order: {Order}", order);
    }
}