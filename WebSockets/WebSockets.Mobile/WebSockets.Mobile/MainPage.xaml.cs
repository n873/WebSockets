using System;
using System.Collections.Generic;
using System.ComponentModel;
using WebSockets.Domain.Order;
using WebSockets.Domain.Rate;
using WebSockets.Domain.Rate.Enum;
using WebSockets.Mobile.Service.Order;
using WebSockets.Mobile.Service.Rate;
using Xamarin.Forms;

namespace WebSockets.Mobile
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        readonly OrderService OrderService = new OrderService();
        readonly RateService RateService = new RateService();

        public MainPage()
        {
            InitializeComponent();

            OrderService.OrderUpdateRecieved += (s, o) =>
            {
                OrderMessage.Text = string.Empty;

                foreach (var update in o.OrderUpdates)
                {
                    OrderMessage.Text += $"{update.Id} - {update.Status} {System.Environment.NewLine} ";
                }
            };

            RateService.RateUpdateReceived += (s, o) =>
            {
                RateMessage.Text = string.Empty;

                foreach (var update in o.RateUpdates)
                {
                    RateMessage.Text += $"{update.Currency} : {update.Value} - {update.Country} {System.Environment.NewLine}";
                }
            };
        }

        private void GetOrderStatusUpdate(object sender, EventArgs e)
        {
            OrderService.GetOrderStatusUpdates(new OrderUpdateRequest(new List<OrderUpdate> { new OrderUpdate("1"), new OrderUpdate("2") }));
        }

        private void GetRateUpdate(object sender, EventArgs e)
        {
            RateService.GetRateUpdates(new RateRequest(new List<ExchangeRate> { new ExchangeRate(Currency.USD), new ExchangeRate(Currency.ZAR) }));
        }
    }
}
