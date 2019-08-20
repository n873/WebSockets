using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WebSockets.Api.Proxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseUrls("http://*:7000", "https://*:7001")
                .UseStartup<Startup>();
    }
}