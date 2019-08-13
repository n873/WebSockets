using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace WebSockets.Api
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ProxyMiddleware
    {
        private const int DefaultWebSocketBufferSize = 4096;

        private readonly RequestDelegate _next;
        private readonly ProxyOptions _options;
        private const string _urlToCallKey = "urlToCall";

        protected internal static readonly string[] NotForwardedWebSocketHeaders = { "Connection", "Host", "Upgrade", "Sec-WebSocket-Key", "Sec-WebSocket-Version" };

        public ProxyMiddleware(RequestDelegate next, IOptions<ProxyOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Value.Scheme == null)
            {
                throw new ArgumentException("Options parameter must specify scheme.", nameof(options));
            }
            if (!options.Value.Host.HasValue)
            {
                throw new ArgumentException("Options parameter must specify host.", nameof(options));
            }

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value;
        }

        public Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var uri = new Uri(UriHelper.BuildAbsolute(_options.Scheme, _options.Host, _options.PathBase, context.Request.Path, context.Request.QueryString.Add(_options.AppendQuery)));
            return context.ProxyRequest(uri);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddlewareClassTemplate(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ProxyMiddleware>();
        }
    }
}
