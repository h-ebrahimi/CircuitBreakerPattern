using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace CircuitBreakerPattern
{
    public static class Extensions
    {
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                //.AdvancedCircuitBreakerAsync()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
