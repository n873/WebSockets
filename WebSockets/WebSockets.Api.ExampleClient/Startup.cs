using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebSockets.Common;

namespace WebSockets.Api.ExampleClient
{
    public class Startup
    {
        private const string _ratesWebSocketPath = "/ratenotify";
        private const string _orderStatusWebSocketPath = "/ordernotify";
        
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddWebSocketManager();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }

            app.UseWebSockets();

            app.MapWebSocketManager(_ratesWebSocketPath, serviceProvider.GetService<WebSocketHandler.RatesMessageHandler>());
            app.MapWebSocketManager(_orderStatusWebSocketPath, serviceProvider.GetService<WebSocketHandler.OrderStatusMessageHandler>());

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}