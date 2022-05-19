using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using functionsintro.Models;

namespace Company.Function
{
    public static class ServiceBusSender
    {
        [FunctionName("ServiceBusSender")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] Order order,
            [ServiceBus("InboundOrders", Connection = "ServiceBusConnectionString")] IAsyncCollector<Order> sbOutput,
            ILogger log)
        {
            await sbOutput.AddAsync(order);

            return new OkObjectResult("Success!");
        }
    }
}
