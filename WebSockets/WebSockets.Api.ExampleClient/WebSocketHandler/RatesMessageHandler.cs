using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebSockets.Common;
using WebSockets.Domain.Rate;

namespace WebSockets.Api.ExampleClient.WebSocketHandler
{
    public class RatesMessageHandler : Common.WebSocketHandler
    {
        private static readonly ConcurrentDictionary<string, RateRequest> RateRequests = new ConcurrentDictionary<string, RateRequest>();

        public RatesMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        { }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
        }

        public override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var readBuffer = new ArraySegment<byte>(buffer);
            var str = Encoding.Default.GetString(readBuffer.Array, readBuffer.Offset, result.Count);
            RateRequest rateRequest = str.Deserialze();
            if (rateRequest?.RateRequests != null)
            {
                var socketId = WebSocketConnectionManager.GetId(socket);
                if (!RateRequests.ContainsKey(socketId))
                    RateRequests.TryAdd(socketId, rateRequest);
                else
                    RateRequests[socketId] = rateRequest;
            }
            return Task.CompletedTask;
        }

        public async Task SendUpdateAsync(IEnumerable<ExchangeRate> rateUpdates)
        {
            var rateResponses = new Dictionary<string, RateResponse>();

            foreach (var webSocketRateRequest in RateRequests)
            {
                foreach (var exchangeRate in webSocketRateRequest.Value.RateRequests)
                {
                    foreach (var rateUpdate in rateUpdates)
                    {
                        if (exchangeRate.Currency == rateUpdate.Currency)
                        {
                            var rateRequestSocketId = webSocketRateRequest.Key;
                            var newExchangeRateUpdate = new ExchangeRate(exchangeRate.Currency, rateUpdate.Country, rateUpdate.Value);
                            if (rateResponses.ContainsKey(rateRequestSocketId))
                            {
                                var rateUpdatesResponse = new List<ExchangeRate> { newExchangeRateUpdate };
                                rateUpdatesResponse.AddRange(rateResponses[rateRequestSocketId].RateUpdates);
                                rateResponses[rateRequestSocketId].RateUpdates = rateUpdatesResponse;
                            }
                            else
                                rateResponses.TryAdd( webSocketRateRequest.Key, new RateResponse( new List<ExchangeRate> { newExchangeRateUpdate }));
                        }
                    }
                }
            }

            if (rateResponses.Count > 0)
                foreach(var rateResponse in rateResponses) {
                    await SendMessageAsync(rateResponse.Key, rateResponse.Value.Serialize());
                };
        }
    }
}