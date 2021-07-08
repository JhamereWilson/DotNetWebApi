using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Square;
using Square.Models;
using Square.Apis;
using Square.Exceptions;
using CSWebAPI.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CSWebAPI.Controllers
{
    [ApiController]
    public class InventoryController : ControllerBase
    {

        private SquareClient client;
        private readonly string _locationId;



        private readonly List<OrderLineItem> _clientLineItems;
        public InventoryController(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            // Get environment
            Square.Environment environment = configuration["AppSettings:Environment"] == "sandbox" ?
              Square.Environment.Sandbox : Square.Environment.Production;

            // Build base client
            client = new SquareClient.Builder()
              .Environment(environment)
              .AccessToken(configuration["AppSettings:AccessToken"])
              .Build();

            _locationId = configuration["AppSettings:LocationId"];


        }

        [HttpGet("{id}", Name = "GetInventoryById")]
        public async Task<int> GetInventoryById(string itemVariationId)
        {
            try
            {
                var result = await client.InventoryApi.RetrieveInventoryCountAsync(catalogObjectId: itemVariationId);

                return Int32.Parse(result.Counts[0].Quantity);
            }
            catch (ApiException e)
            {
                Console.WriteLine("Failed to make the request");
                Console.WriteLine($"Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");
                return 0;
            }
        }
    }

}