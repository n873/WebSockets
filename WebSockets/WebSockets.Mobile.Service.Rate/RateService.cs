using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using WebSockets.Domain.Rate;
using WebSockets.Domain.Rate.Enum;

namespace WebSockets.Mobile.Service.Rate
{
    public class RateService : IDisposable
    {
        public event EventHandler<RateResponse> RateUpdateReceived;

        private System.Timers.Timer _timer = new System.Timers.Timer(15000);
        ClientWebSocket ClientWebSocket;
        public RateService()
        {
            _timer.Elapsed += TimerElapsed;
        }

        private async void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            //await SendData({Keep alive message goes here});
        }

        private async Task SendData(RateRequest rateRequest)
        {
            var serialized = rateRequest.Serialize();
            var encoded = Encoding.UTF8.GetBytes(serialized);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);

            await ClientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async void GetRateUpdates(RateRequest rateRequest)
        {
            ClientWebSocket = new ClientWebSocket(); 
            try
            {
                await ClientWebSocket.ConnectAsync(new Uri($"ws://10.0.2.2:7000/ratenotify"), CancellationToken.None);

                await SendData(
                    new RateRequest(
                        new List<ExchangeRate> {
                        new ExchangeRate(Currency.USD),
                        new ExchangeRate(Currency.ZAR)}));

                _timer.Start();

                var readBuffer = new ArraySegment<byte>(new byte[8192]);
                while (ClientWebSocket.State == WebSocketState.Open)
                {
                    RateResponse rateUpdate = null;

                    var result = await ClientWebSocket.ReceiveAsync(readBuffer, CancellationToken.None);
                    var str = Encoding.Default.GetString(readBuffer.Array, readBuffer.Offset, result.Count);
                    rateUpdate = JsonConvert.DeserializeObject<RateResponse>(str);
                    if (rateUpdate != null)
                        RateUpdateReceived?.Invoke(this, rateUpdate);
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
                    await ClientWebSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
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
