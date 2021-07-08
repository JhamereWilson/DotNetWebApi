using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CSWebAPI.Models
{

    public class SquareCatalogItemObject
    {
        [JsonProperty("ParentId")]
        public string Id { get; set; }
        [JsonProperty("Url")]
        public string ImageUrl { get; set; }
        [JsonProperty("ItemName")]
        
        public string Name { get; set; }

        [JsonProperty("CategoryId")]
        public string CategoryId {get; set;}
        [JsonProperty("ItemVariations")]
        public List<SquareCatalogItemVariation> variations { get; set; }



    }
}
