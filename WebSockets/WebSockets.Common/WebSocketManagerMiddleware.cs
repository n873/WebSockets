using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebSockets.Common
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler WebSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            WebSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest) return;

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await WebSocketHandler.OnConnected(socket);

            await Receive(socket, async (result, buffer) =>
            {
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        await WebSocketHandler.ReceiveAsync(socket, result, buffer);
                        return;
                    case WebSocketMessageType.Close:
                        await WebSocketHandler.OnDisconnected(socket);
                        return;
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            try
            {
                var buffer = new byte[1024 * 4];

                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                        cancellationToken: CancellationToken.None);

                    handleMessage(result, buffer);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }
    }
}