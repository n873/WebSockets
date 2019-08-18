using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using WebSockets.Domain;
using WebSockets.Domain.Order;
using WebSockets.Domain.Order.Enum;
using WebSockets.Domain.Rate;

namespace WebSockets.ServiceDaemon
{
    public class WorkerHostedService : BackgroundService
    {
        private const string MediaType = "application/json";
        private const string OrderUpdateEndpoint = "https://localhost:6001/orderupdate";
        private const string RateUpdateEndpoint = "https://localhost:6001/rateupdate";

        private readonly Random Random = new Random();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var newOrderUpdates = GenerateOrderUpdates();
                var serializedOrderUpdatesObject = newOrderUpdates.Serialize();
                var httpOrderUpdatesContent = new StringContent(serializedOrderUpdatesObject, Encoding.UTF8, MediaType);

                var newRateUpdates = GenerateRateUpdates();
                var serializedRateUpdatesObject = newRateUpdates.Serialize();
                var httpRateUpdatesContent = new StringContent(serializedRateUpdatesObject, Encoding.UTF8, MediaType);

                var httpClient = new HttpClient();

                var orderUpdateResponse = await httpClient.PostAsync(OrderUpdateEndpoint, httpOrderUpdatesContent, CancellationToken.None);
                orderUpdateResponse.EnsureSuccessStatusCode();
                var orderUpdateResponseBody = await orderUpdateResponse.Content.ReadAsStringAsync();

                var rateUpdateResponse = await httpClient.PostAsync(RateUpdateEndpoint, httpRateUpdatesContent, CancellationToken.None);
                rateUpdateResponse.EnsureSuccessStatusCode();
                var rateUpdateResponseBody = await rateUpdateResponse.Content.ReadAsStringAsync();

                Thread.Sleep(5000);
            }
        }
        
        private IEnumerable<OrderUpdate> GenerateOrderUpdates()
        {
            return new List<OrderUpdate> {
                new OrderUpdate("1", (OrderStatus) Random.Next(0, Enum.GetNames(typeof(OrderStatus)).Length)),
                new OrderUpdate("2", (OrderStatus) Random.Next(0, Enum.GetNames(typeof(OrderStatus)).Length))
            };
        }

        private IEnumerable<ExchangeRate> GenerateRateUpdates()
        {
            return new List<ExchangeRate> {
                new ExchangeRate(Domain.Rate.Enum.Currency.USD, Domain.Rate.Enum.Country.America, Random.NextDouble() * (20.11 - 11.20) + 11.20),
                new ExchangeRate(Domain.Rate.Enum.Currency.ZAR, Domain.Rate.Enum.Country.SouthAfrica, Random.NextDouble() * (20.11 - 11.20) + 11.20)
            };
        }
    }
}  