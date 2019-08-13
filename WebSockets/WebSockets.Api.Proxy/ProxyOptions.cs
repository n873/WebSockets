﻿using Microsoft.AspNetCore.Http;

namespace WebSockets.Api
{
    public class ProxyOptions
    {
        public string Scheme { get; set; }
        public HostString Host { get; set; }
        public PathString PathBase { get; set; }
        public QueryString AppendQuery { get; set; }
    }
}
