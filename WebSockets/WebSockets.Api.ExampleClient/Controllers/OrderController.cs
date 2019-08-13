using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebSockets.Api.ExampleClient.WebSocketHandler;
using WebSockets.Domain.Order;

namespace WebSockets.Api.ExampleClient.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderNotifyController : ControllerBase
    {
        private readonly OrderStatusMessageHandler _orderStatusMessageHandler;
        
        public OrderNotifyController(OrderStatusMessageHandler orderStatusMessageHandler)
        {
            _orderStatusMessageHandler = orderStatusMessageHandler;
        }
        
        [HttpPost]
        public async Task OrderStatusUpdate([FromBody]IEnumerable<OrderUpdate> orderUpdates)
        {
            await _orderStatusMessageHandler.SendUpdateAsync(orderUpdates);
        }

        [HttpGet]
        public string Ping() => "Pong";
    }
}