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
    public class CatalogController : ControllerBase
    {

        private SquareClient client;
        private readonly string _locationId;



        private readonly List<OrderLineItem> _clientLineItems;
        public CatalogController(Microsoft.Extensions.Configuration.IConfiguration configuration)
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

        [HttpGet("catalog")]

        public async Task<IActionResult> GetAllItems()
        {
            var newList = new List<String>();
            try
            {
                var result = await client.CatalogApi.ListCatalogAsync(types: "ITEM,IMAGE,ITEM_VARIATION");

                var imageList = result.Objects.Where(o => o.Type == "IMAGE").ToList();

                var itemList = result.Objects.Where(o => o.Type == "ITEM").ToList();

                var variationList = result.Objects.Where(o => o.Type == "ITEM_VARIATION").ToList();


                var filteredVariations = (from variation in variationList
                                          join item in itemList on variation.ItemVariationData.ItemId equals item.Id
                                          select new SquareCatalogItemVariation { ParentId = item.Id, Id = variation.Id, Price = ((int)variation.ItemVariationData.PriceMoney.Amount), VariationName = variation.ItemVariationData.Name }).ToList();


                var filteredList =
                // identify data sources
                (from image in imageList

                     //join relational data
                 join item in itemList on image.Id equals item.ImageId
                 //declare new object with query
                 select new SquareCatalogItemObject { Id = item.Id, ImageUrl = image.ImageData.Url, variations = filteredVariations, Name = item.ItemData.Name }).ToList();



                
                string output = JsonConvert.SerializeObject(filteredList);
                // foreach (var o in filteredList)
                // {
                //     newList.Add(o);
                //     string output = JsonConvert.SerializeObject(newList);
                    

                // }
                return Ok(output);

            }
            catch (ApiException e)
            {
                Console.WriteLine("Failed to make the request");
                Console.WriteLine($"Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");
                return BadRequest();
            }
        }

        [HttpGet("{id}", Name = "image-info")]

        public async Task<IActionResult> GetImageUrlById(string id)
        {
            try
            {

                var result = await client.CatalogApi.RetrieveCatalogObjectAsync(objectId: id, includeRelatedObjects: false);
                return Ok(result);

            }
            catch (ApiException e)
            {
                Console.WriteLine("Failed to make the request");
                Console.WriteLine($"Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");
                return BadRequest();
            }
        }

    }

}
