using System.Collections.Generic;

namespace WebSockets.Domain.Rate
{
    public class RateRequest
    {
        public RateRequest() {}
        public RateRequest(IEnumerable<ExchangeRate> rateRequests) { RateRequests = rateRequests; }

        public IEnumerable<ExchangeRate> RateRequests { get; set; }
    }
}
