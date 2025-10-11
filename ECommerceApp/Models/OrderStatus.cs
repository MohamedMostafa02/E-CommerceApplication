using System.Reflection.Metadata;
using System.Text.Json.Serialization;
namespace ECommerceApp.Models
{
    //Enum to represent the status of an order
    // The JsonStringEnumConverter ensures that enums are serialized as thier string names
    // instead of interger values, enhancing readability.
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Canceled = 5
    }
}
