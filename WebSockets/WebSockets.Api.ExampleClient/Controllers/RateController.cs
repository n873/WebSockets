using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSockets.Api.ExampleClient.WebSocketHandler;
using WebSockets.Domain.Rate;

namespace WebSockets.Api.ExampleClient.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RateUpdateController : ControllerBase
    {
        private readonly RatesMessageHandler RateMessageHandler;

        public RateUpdateController(RatesMessageHandler ratesMessageHandler)
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
