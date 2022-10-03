using Newtonsoft.Json;

namespace OrderReservator.Models;

public class OrderItem
{
    [JsonProperty(Required = Required.Always)]
    public int Id { get; set; }

    [JsonProperty(Required = Required.Always)]
    public int Quantity { get; set; }
}
