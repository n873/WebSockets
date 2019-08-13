using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSockets.Api.ExampleClient.WebSocketHandler;
using WebSockets.Domain.Rate;

namespace WebSockets.Api.ExampleClient.Controllers
{
    [Route("controller")]
    [ApiController]
    public class RateController : ControllerBase
    {
        private readonly RatesMessageHandler RateMessageHandler;

        public RateController(RatesMessageHandler ratesMessageHandler)
        {
            RateMessageHandler = ratesMessageHandler;
        }

        [HttpPost]
        public async Task RateUpdate([FromBody]IEnumerable<ExchangeRate> rateUpdates) {
            await RateMessageHandler.SendUpdateAsync(rateUpdates);
        }

        [HttpGet]
        public string Ping() => "Pong";
    }
}
