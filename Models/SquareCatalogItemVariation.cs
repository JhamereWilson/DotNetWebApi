using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CSWebAPI.Models
{

    public class SquareCatalogItemVariation
    {
        [JsonProperty("ParentId")]
        public string ParentId;
        [JsonProperty("VariationId")]
        public string Id;
        [JsonProperty("Price")]
        public int Price;
        [JsonProperty("VariationName")]
        public string VariationName;




    }
}
