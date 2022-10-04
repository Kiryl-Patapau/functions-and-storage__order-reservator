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
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "reserve")] HttpRequest request,
        IBinder binder,
        ILogger logger)
    {
        using var bodyReader = new StreamReader(request.Body);
        var body = await bodyReader.ReadToEndAsync();

        try
        {
            var items = JsonConvert.DeserializeObject<OrderItem[]>(body);
            
            // Dynamic binding is used to avoid creating empty blobs in case of 400 (BadRequest)
            var order = JsonConvert.SerializeObject(items);
            var blobAttribute = new BlobAttribute("orders/{datetime:yyyy-MM-dd}/{rand-guid}.json")
            {
                Access = FileAccess.Write,
                Connection = "OrdersStorage"
            };
            using var orderStream = binder.Bind<Stream>(blobAttribute);
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
