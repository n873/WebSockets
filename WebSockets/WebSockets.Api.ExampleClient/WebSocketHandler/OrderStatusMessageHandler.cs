using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebSockets.Common;
using WebSockets.Domain.Order;

namespace WebSockets.Api.ExampleClient.WebSocketHandler
{
    public class OrderStatusMessageHandler : Common.WebSocketHandler
    {
        private static readonly ConcurrentDictionary<string, OrderUpdateRequest> OrderUpdateRequests = new ConcurrentDictionary<string, OrderUpdateRequest>();

        public OrderStatusMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        { }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
        }

        public override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var str = Encoding.Default.GetString(buffer, 0, result.Count);
            OrderUpdateRequest orderRequest = str.Deserialze();
            if(orderRequest?.OrderUpdateRequests != null)
            {
                var socketId = WebSocketConnectionManager.GetId(socket);
                if (!OrderUpdateRequests.ContainsKey(socketId))
                    OrderUpdateRequests.TryAdd(socketId, orderRequest);
                else
                    OrderUpdateRequests[socketId] = orderRequest;
            }
            return Task.CompletedTask;
        }

        public async Task SendUpdateAsync(IEnumerable<OrderUpdate> orderUpdates)
        {
            var orderResponses = new Dictionary<string, OrderResponse>();

            foreach (var webSocketOrderRequest in OrderUpdateRequests) {
                foreach (var orderUpdate in webSocketOrderRequest.Value.OrderUpdateRequests)
                {
                    foreach (var update in orderUpdates) {
                        if (orderUpdate.Id == update.Id) {
                            var orderRequestSocketId = webSocketOrderRequest.Key;
                            var newOrderUpdate = new OrderUpdate(orderUpdate.Id, update.Status);
                            if (orderResponses.ContainsKey(orderRequestSocketId))
                            {
                                var orderUpdatesResponse = new List<OrderUpdate> { newOrderUpdate };
                                orderUpdatesResponse.AddRange(orderResponses[orderRequestSocketId].OrderUpdates);
                                orderResponses[orderRequestSocketId].OrderUpdates = orderUpdatesResponse;
                            }
                            else
                                orderResponses.TryAdd(webSocketOrderRequest.Key, new OrderResponse(new List<OrderUpdate> { newOrderUpdate }));
                        }
                    }
                }
            }

            if (orderResponses.Count > 0)
                foreach (var orderResponse in orderResponses) {
                    await SendMessageAsync(orderResponse.Key, orderResponse.Value.Serialize());
                }
        }
        
    }
}