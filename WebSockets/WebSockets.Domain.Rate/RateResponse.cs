using System.Collections.Generic;

namespace WebSockets.Domain.Rate
{
    public class RateResponse
    {
        public RateResponse() { }
        public RateResponse(IEnumerable<ExchangeRate> rateUpdates)
        {
            RateUpdates = rateUpdates;
        }
        public IEnumerable<ExchangeRate> RateUpdates { get; set; }
    }
}
