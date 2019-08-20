using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSockets.Domain.Order;
using System.Collections.Generic;

namespace WebSockets.Mobile.Service.Order
{
    public class OrderService : IDisposable
    {
        public event EventHandler<OrderResponse> OrderUpdateRecieved;

        private System.Timers.Timer _timer = new System.Timers.Timer(15000);
        ClientWebSocket ClientWebSocket;

        public OrderService()
        {
            _timer.Elapsed += _timer_Elapsed;
        }

        private async void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        private async Task SendData(OrderUpdateRequest data)
        {
            var serialized = data.Serialize();
            var encoded = Encoding.UTF8.GetBytes(serialized);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);

            await ClientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async void GetOrderStatusUpdates(OrderUpdateRequest order)
        {

            ClientWebSocket = new ClientWebSocket();
            try
            {

                await ClientWebSocket.ConnectAsync(new Uri($"wss://10.0.2.2:7001/ordernotify"), CancellationToken.None);

                await SendData(new OrderUpdateRequest(new List<OrderUpdate> { new OrderUpdate("1"), new OrderUpdate("2") }));

                _timer.Start();

                var readBuffer = new ArraySegment<byte>(new byte[8192]);
                while (ClientWebSocket.State == WebSocketState.Open)
                {
                    OrderResponse orderUpdate = null;

                    var result = await ClientWebSocket.ReceiveAsync(readBuffer, CancellationToken.None);
                    var str = Encoding.Default.GetString(readBuffer.Array, readBuffer.Offset, result.Count);
                    orderUpdate = JsonConvert.DeserializeObject<OrderResponse>(str);
                    if (orderUpdate != null)
                    {
                        OrderUpdateRecieved?.Invoke(this, orderUpdate);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                Debug.Write("WebSocket closed");
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }

        public async void StopLoadingData()
        {
            try
            {
                if (ClientWebSocket.State != WebSocketState.Closed)
                    await ClientWebSocket.CloseAsync(WebSocketCloseStatus.Empty, String.Empty, CancellationToken.None);
            }
            finally
            {
                ClientWebSocket.Dispose();
                _timer.Stop();
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
            if (ClientWebSocket != null)
            {
                ClientWebSocket.Dispose();
                ClientWebSocket = null;
            }
        }
    }
}
