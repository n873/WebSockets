using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebSockets.Domain.Order
{
    public static class OrderExtension
    {
        public static string Serialize(this OrderUpdateRequest order)
            => JsonConvert.SerializeObject(order);

        public static string Serialize(this OrderResponse order)
            => JsonConvert.SerializeObject(order);

        public static string Serialize(this OrderUpdate order)
            => JsonConvert.SerializeObject(order);

        public static string Serialize(this IEnumerable<OrderUpdate> orderUpdates)
            => JsonConvert.SerializeObject(orderUpdates);

        public static OrderUpdateRequest Deserialze(this string order)
            => JsonConvert.DeserializeObject<OrderUpdateRequest>(order);
    }
}
