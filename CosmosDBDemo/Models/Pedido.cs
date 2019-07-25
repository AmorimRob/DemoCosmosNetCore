using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBDemo.Models
{
    public class Pedido
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Descrição { get; set; }
        public string Preço { get; set; }

    }
}
