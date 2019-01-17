using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Ringo.Common.Models;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Azure.Documents.Client;
using System;

namespace Ringo.Functions.User
{
    public class UserCommands
    {
        private readonly DocumentClient _client;
        private string EndpointUrl = Environment.GetEnvironmentVariable("CosmosEndpoint");
        private string AuthorizationKey = Environment.GetEnvironmentVariable("CosmosEndpoint");
        private const string DatabaseId = "ringodb";
        private const string CollectionId = "graph";


        [FunctionName("GetUser")]
        public static async Task<IActionResult> GetUser(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string id = req.Query["id"];

            return id != null
                ? (ActionResult)new OkResult()
                : new BadRequestObjectResult("Please pass userId in the query params");
        }

        [FunctionName("CreateUser")]
        public static async Task<IActionResult> CreateUser(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "ringodb",
                collectionName: "graph",
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<UserEntity> userOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserEntity user = JsonConvert.DeserializeObject<UserEntity>(requestBody);

            try
            {
                await userOut.AddAsync(user);
            }
            catch (System.Exception)
            {
                return new BadRequestResult();
                //throw;
            }

            return (ActionResult)new OkResult();
        }

        [FunctionName("UpdateUser")]
        public static async Task<IActionResult> UpdateUser(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserEntity user = JsonConvert.DeserializeObject<UserEntity>(requestBody);

            try
            {
                //todo
            }
            catch (System.Exception)
            {
                return new BadRequestResult();
                //throw;
            }

            return (ActionResult)new OkResult();
        }
    }
}
