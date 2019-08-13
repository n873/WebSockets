using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WebSockets.Api
{
    public static class ProxyExtensions
    {
        public static void RunProxy(this IApplicationBuilder app, Uri baseUri)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (baseUri == null)
            {
                throw new ArgumentNullException(nameof(baseUri));
            }

            var options = new ProxyOptions
            {
                Scheme = baseUri.Scheme,
                Host = new HostString(baseUri.Authority),
                PathBase = baseUri.AbsolutePath,
                AppendQuery = new QueryString(baseUri.Query)
            };
            app.UseMiddleware<ProxyMiddleware>(Options.Create(options));
        }

        public static void RunProxy(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseMiddleware<ProxyMiddleware>();
        }

        public static void RunProxy(this IApplicationBuilder app, ProxyOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            app.UseMiddleware<ProxyMiddleware>(Options.Create(options));
        }

        public static async Task ProxyRequest(this HttpContext context, Uri destinationUri)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (destinationUri == null)
            {
                throw new ArgumentNullException(nameof(destinationUri));
            }

            if (context.WebSockets.IsWebSocketRequest)
            {
                await context.AcceptProxyWebSocketRequest(destinationUri.ToWebSocketScheme());
            }
            else
            {
                var proxyService = context.RequestServices.GetRequiredService<ProxyService>();

                using (var requestMessage = context.CreateProxyHttpRequest(destinationUri))
                {
                    var prepareRequestHandler = proxyService.Options.PrepareRequest;
                    if (prepareRequestHandler != null)
                    {
                        await prepareRequestHandler(context.Request, requestMessage);
                    }

                    using (var responseMessage = await context.SendProxyHttpRequest(requestMessage))
                    {
                        await context.CopyProxyHttpResponse(responseMessage);
                    }
                }
            }
        }
    }
}
