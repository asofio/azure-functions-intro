using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using FunctionsIntro.Models;

using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;

namespace Sofio.Function
{
    public static class OrderHandler
    {
        private const string CosmosDatabase = "Tax";
        private const string CosmosContainer = "SalesTax";
        private const string OutboundServiceBusQueue = "OrderResult";

        [FunctionName("OrderHandler")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Read data from POST body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Order order = JsonConvert.DeserializeObject<Order>(requestBody);

            // Read document from Cosmos DB
            CosmosClient cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("CosmosDBConnectionString"));
            var db = cosmosClient.GetDatabase(CosmosDatabase);
            var container = db.GetContainer(CosmosContainer);
            var salesTaxResult = await container.ReadItemAsync<SalesTax>(order.StateAbbreviation, new PartitionKey(order.StateAbbreviation));
            var salesTax = salesTaxResult.Resource;

            // Calculate total price
            decimal totalPrice = 0;
            foreach (var item in order.Items)
                totalPrice += item.Price * item.Quantity;

            // Apply sales tax
            totalPrice = totalPrice + (totalPrice * salesTax.Percentage);

            // Instantiate outbound OrderResult model
            var orderResult = new OrderResult { TotalPrice = totalPrice };

            // Query API to retrieve product name
            using (var httpClient = new HttpClient()) {

                foreach(var item in order.Items) {
                    var itemInfo = await httpClient.GetAsync($"https://api.sampleapis.com/coffee/hot/{item.ItemId}");

                    if(itemInfo.IsSuccessStatusCode) {
                        var json = await itemInfo.Content.ReadAsStringAsync();
                        var productResult = JsonConvert.DeserializeObject<ProductResult>(json);
                        
                        orderResult.Items.Add(new OrderResultItem
                        {
                            ItemId = item.ItemId,
                            ProductName = productResult.Title,
                            Price = item.Price,
                            Quantity = item.Quantity
                        });
                    }
                    else {
                        log.LogInformation($"Failed to pull product info for item {item.ItemId}");
                    }
                }

            }

            // Send Message to Service Bus
            ServiceBusClient serviceBusClient = new ServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusConnectionString"));
            var queueClient = serviceBusClient.CreateSender(OutboundServiceBusQueue);
            var message = new ServiceBusMessage(JsonConvert.SerializeObject(orderResult));
            message.ContentType = "application/json";
            await queueClient.SendMessageAsync(message);
        
            return new OkObjectResult($"The sales tax for {salesTax.State} is {salesTax.Percentage}.");
        }
    }
}