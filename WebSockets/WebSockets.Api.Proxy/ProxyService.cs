﻿using System;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace WebSockets.Api
{
    public class ProxyService
    {
        public ProxyService(IOptions<SharedProxyOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options.Value;
            Client = new HttpClient(Options.MessageHandler ?? new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false });
        }

        public SharedProxyOptions Options { get; private set; }
        internal HttpClient Client { get; private set; }
    }
}
