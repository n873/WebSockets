using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebSockets.Domain.Rate
{
    public static class RateExtension
    {
        public static string Serialize(this RateRequest order)
            => JsonConvert.SerializeObject(order);

        public static string Serialize(this RateResponse order)
            => JsonConvert.SerializeObject(order);

        public static string Serialize(this IEnumerable<ExchangeRate> exchangeRates)
            => JsonConvert.SerializeObject(exchangeRates);

        public static RateRequest Deserialze(this string order)
            => JsonConvert.DeserializeObject<RateRequest>(order);
    }
}
