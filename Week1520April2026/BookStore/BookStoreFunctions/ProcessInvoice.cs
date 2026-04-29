using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BookStoreFunctions;

public class ProcessInvoice
{
    private readonly ILogger<ProcessInvoice> _logger;
    public ProcessInvoice(ILogger<ProcessInvoice> logger) => _logger = logger;

    [Function("ProcessInvoice")]
    public void Run([BlobTrigger("invoices/{name}", Connection = "AzureWebJobsStorage")] Stream file, string name)
    {
        _logger.LogInformation("Invoice uploaded: {Name}, Size: {Size} bytes", name, file.Length);
    }
}