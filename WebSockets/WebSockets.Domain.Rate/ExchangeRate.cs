using WebSockets.Domain.Rate.Enum;

namespace WebSockets.Domain.Rate
{
    public class ExchangeRate
    {
        public ExchangeRate() { }

        public ExchangeRate(Currency currency) 
        {
            Currency = currency;
        }

        public ExchangeRate(Currency currency, Country country)
        {
            Currency = currency; Country = country;
        }

        public ExchangeRate(Currency currency, Country country, double value)
        {
            Currency = currency; Country = country; Value = value;
        }

        public Currency Currency { get; set; }
        public Country Country { get; set; }
        public double Value { get; set; }
    }
}
