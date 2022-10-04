using System.Collections.Generic;
using Newtonsoft.Json;

namespace OrderReservator.Models;

public class Order
{
    [JsonProperty(Required = Required.Always)]
    public string BuyerId { get; set; }

    [JsonProperty(Required = Required.Always)]
    public IEnumerable<OrderItem> Items { get; set; }
}
