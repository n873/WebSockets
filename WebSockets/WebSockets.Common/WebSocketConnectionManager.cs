using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebSockets.Common
{
    public class WebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private const string NormalClosureStatusDescription = "Closed by the WebSocketManager";

        public WebSocket GetSocketById(string id)
            => _sockets.FirstOrDefault(keyValuePair => keyValuePair.Key == id).Value;

        public ConcurrentDictionary<string, WebSocket> GetAll()
            => _sockets;

        public string GetId(WebSocket socket)
            => _sockets.FirstOrDefault(keyValuePair => keyValuePair.Value == socket).Key;

        public IEnumerable<WebSocket> GetIdStartingWith(string searchCriteria)
            => _sockets.Where(keyValuePair => keyValuePair.Key.StartsWith(searchCriteria))
                .Select(keyValuePair => keyValuePair.Value);
        
        public void AddSocket(WebSocket socket)
        {
            var socketId = CreateConnectionId();
            while (!_sockets.TryAdd(socketId, socket))
                socketId = CreateConnectionId();
        }

        public async Task RemoveSocket(string id)
        {
            _sockets.TryRemove(id, out var socket);

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                statusDescription: NormalClosureStatusDescription,
                cancellationToken: CancellationToken.None);
        }

        private string CreateConnectionId()
            => Guid.NewGuid().ToString();
    }
}