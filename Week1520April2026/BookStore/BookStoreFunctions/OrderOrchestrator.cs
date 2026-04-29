using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BookStoreFunctions;

public class OrderOrchestrator
{
    [Function("OrderOrchestrator")]
    public async Task RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var orderId = context.GetInput<string>() ?? "Unknown";
        await context.CallActivityAsync("CheckStock", orderId);
        await context.CallActivityAsync("ReserveInventory", orderId);
        await context.CallActivityAsync("GenerateInvoice", orderId);
        await context.CallActivityAsync("SendEmail", orderId);
    }

    [Function("CheckStock")]
    public void CheckStock([ActivityTrigger] string orderId, ILogger log)
    {
        log.LogInformation("Stock checked for order {OrderId}", orderId);
    }

    [Function("ReserveInventory")]
    public void ReserveInventory([ActivityTrigger] string orderId, ILogger log)
    {
        log.LogInformation("Inventory reserved for order {OrderId}", orderId);
    }

    [Function("GenerateInvoice")]
    public void GenerateInvoice([ActivityTrigger] string orderId, ILogger log)
    {
        log.LogInformation("Invoice generated for order {OrderId}", orderId);
    }

    [Function("SendEmail")]
    public void SendEmail([ActivityTrigger] string orderId, ILogger log)
    {
        log.LogInformation("Email sent for order {OrderId}", orderId);
    }

    [Function("StartOrderWorkflow")]
    public async Task<HttpResponseData> StartWorkflow(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client)
    {
        string orderId = await req.ReadAsStringAsync() ?? "ORD-001";
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync("OrderOrchestrator", orderId);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { InstanceId = instanceId, Status = "Started" });
        return response;
    }
}