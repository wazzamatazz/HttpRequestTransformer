using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AddAccessTokenExample {
    public class Startup {
        
        public void ConfigureServices(IServiceCollection services) {
            // Register our HTTP client and add a request transformer that will add an 
            // Authorization header to outgoing requests.
            services
                .AddHttpClient("Test", options => {
                    options.BaseAddress = new Uri("https://some-remote-site.com");
                })
                .AddHttpMessageHandler(() => new Jaahas.Http.HttpRequestTransformHandler(AddBearerTokenToRequest));
        }

        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.Run(async (context) => {
                var factory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
                var client = factory.CreateClient("Test");

                // Create the request and add the calling user to its state dictionary.
                var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com")
                    .AddStateProperty(context.User);

                try {
                    var response = await client.SendAsync(request, context.RequestAborted);
                    if (!response.IsSuccessStatusCode) {
                        await context.Response.WriteAsync("Failed to get data!");
                    }
                    else {
                        var data = await response.Content.ReadAsStringAsync();
                        await context.Response.WriteAsync($"Data received: {data}");
                    }
                }
                catch {
                    await context.Response.WriteAsync("Failed to get data!");
                }
            });
        }


        private static Task AddBearerTokenToRequest(HttpRequestMessage request, CancellationToken cancellationToken) {
            var principal = request.GetStateProperty<ClaimsPrincipal>();
            if (principal?.Identity?.IsAuthenticated ?? false) {
                // Obviously you'd need to do a lookup to get the access token for the calling 
                // user in a real application.
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Guid.NewGuid().ToString());
            }
            return Task.CompletedTask;
        }

    }
}
