using System.Collections.Generic;

namespace WebSockets.Domain.Order
{
    public class OrderUpdateRequest
    {
        public OrderUpdateRequest() { }
        public OrderUpdateRequest(IEnumerable<OrderUpdate> orderUpdateRequests) { OrderUpdateRequests = orderUpdateRequests; }

        public IEnumerable<OrderUpdate> OrderUpdateRequests { get; set; }
    }
}
