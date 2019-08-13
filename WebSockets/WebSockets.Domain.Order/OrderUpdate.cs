using WebSockets.Domain.Order.Enum;

namespace WebSockets.Domain.Order
{
    public class OrderUpdate
    {
        public OrderUpdate() { }
        public OrderUpdate(string id) { Id = id; }
        public OrderUpdate(string id, OrderStatus status) { Id = id; Status = status; }

        public string Id { get; set; }
        public OrderStatus Status { get; set; }
    }
}
