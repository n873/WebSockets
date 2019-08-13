using System.Collections.Generic;

namespace WebSockets.Domain.Order
{
    public class OrderResponse
    {
        public OrderResponse() { }
        public OrderResponse(IEnumerable<OrderUpdate> orderUpdates)
        {
            OrderUpdates = orderUpdates;
        }
        public IEnumerable<OrderUpdate> OrderUpdates { get; set; }
    }
}
