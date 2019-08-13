using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WebSockets.Api.ExampleClient
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseUrls("http://*:6000","https://*:6001")
                .UseStartup<Startup>();
    }
}