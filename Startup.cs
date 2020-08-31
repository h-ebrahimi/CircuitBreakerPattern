using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Polly;
using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Http;

namespace CircuitBreakerPattern
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllers();
            services.AddHttpContextAccessor();

            var basicCircuitBreakerPolicy = Policy
                    .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.RequestTimeout || r.StatusCode == HttpStatusCode.GatewayTimeout || r.StatusCode == HttpStatusCode.NotFound || r.StatusCode == HttpStatusCode.InternalServerError)
                    .AdvancedCircuitBreakerAsync(0.25, TimeSpan.FromSeconds(60), 3,                    TimeSpan.FromSeconds(10), OnBreak, OnReset, OnHalfOpen);

            services.AddHttpClient("Dogs")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(basicCircuitBreakerPolicy);
        }

        private void OnHalfOpen()
        {
            Console.WriteLine("Circuit in test mode, one request will be allowed.");
        }

        private void OnReset()
        {
            Console.WriteLine("Circuit closed, requests flow normally.");
        }

        private void OnBreak(DelegateResult<HttpResponseMessage> result, TimeSpan ts)
        {
            Console.WriteLine("Circuit cut, requests will not flow.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
