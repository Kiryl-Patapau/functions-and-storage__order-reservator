using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OrderReservator.Models;
using Microsoft.Extensions.Logging;

namespace OrderReservator.Functions;

public static class ReserveFunction
{
    [FunctionName("ReserveFunction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "reserve")] HttpRequest request,
        [Blob("orders/{rand-guid}.json", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream orderStream,
        ILogger logger)
    {
        using var bodyReader = new StreamReader(request.Body);
        var body = await bodyReader.ReadToEndAsync();

        try
        {
            var items = JsonConvert.DeserializeObject<OrderItem[]>(body);

            var order = JsonConvert.SerializeObject(items);
            using var writer = new StreamWriter(orderStream);
            await writer.WriteLineAsync(order);
        }
        catch (JsonException exception)
        {
            logger.LogWarning(exception, "Invalid request body is received by {function}.", nameof(ReserveFunction));
            return new BadRequestResult();
        }

        return new OkResult();
    }
}
