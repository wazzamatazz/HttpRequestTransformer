# HttpRequestTransformer

Contains an `DelegatingHandler` ([HttpRequestPipelineHandler](./src/HttpRequestTransformer/HttpRequestPipelineHandler.cs)) that can be used to inspect or modify outgoing HTTP requests prior to sending, and/or to inspect or modify received HTTP response messages. An example use case includes a backchannel HTTP client in a server-side app where you need to attach the authorization token for the calling user to an outgoing request.


# Getting Started

Add the `Jaahas.HttpRequestTransformer` [NuGet package](https://www.nuget.org/packages/Jaahas.HttpRequestTransformer) to your project.


# Example: Adding a Header to Outgoing Requests

In this example, we'll attach a bearer token to outgoing requests made by a backchannel client in an ASP.NET Core application. First, add an [HttpRequestPipelineHandler](./src/HttpRequestTransformer/HttpRequestPipelineHandler.cs) to the request pipeline for your `HttpClient`:

```csharp
public void ConfigureServices(IServiceCollection services) {
    services
        .AddHttpClient("Test", options => {
            options.BaseAddress = new Uri("https://some-remote-site.com");
        })
        .AddHttpMessageHandler(() => new Jaahas.Http.HttpRequestPipelineHandler(AddBearerTokenToRequest));
}
```

Next, implement the function that will retrieve the access token for a given `ClaimsPrincipal`:

```csharp
private static Task AddBearerTokenToRequest(HttpRequestMessage request, CancellationToken cancellationToken) {
    var principal = request.GetStateProperty<ClaimsPrincipal>();
    if (principal?.Identity?.IsAuthenticated ?? false) {
        string token;

        // Plug in your own logic here to get the actual token for the principal...

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    return Task.CompletedTask;
}
```

Finally, in your code that will use the client, add a `ClaimsPrincipal` to the outgoing request's properties prior to sending:

```csharp
[ApiController]
[Authorize]
public class MyController : ControllerBase {

    private readonly HttpClient _httpClient;


    public MyController(IHttpClientFactory factory) {
        _httpClient = factory.CreateClient("Test");
    }


    public async Task<IActionResult> GetDataFromRemoteSite(CancellationToken cancellationToken) {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/data").AddStateProperty(User);
        var response = await _httpClient.SendAsync(request, cancellationToken);

        // Process the response and return the result to the caller.
    } 

}
```


# Example: Logging Detailed Response Information

In this example, we'll create an HTTP handler that will write detailed information about an HTTP request and response to the console:

```csharp
public void ConfigureServices(IServiceCollection services) {
    services
        .AddHttpClient("Test", options => {
            options.BaseAddress = new Uri("https://some-remote-site.com");
        })
        .AddHttpMessageHandler(() => new Jaahas.Http.HttpRequestPipelineHandler(LogResponseDetails));
}


private static Task LogResponseDetails(HttpResponseMessage response, CancellationToken cancellationToken) {
    // Method and endpoint.
    Console.WriteLine($"{response.RequestMessage.Method} {response.RequestMessage.RequestUri}");
    Console.WriteLine();

    // Request headers.
    foreach (var item in response.RequestMessage.Headers) {
        Console.WriteLine($"{item.Key}: {string.Join(", ", item.Value)}");
    }
    if (response.RequestMessage.Content != null) {
        foreach (var item in response.RequestMessage.Content.Headers) {
            Console.WriteLine($"{item.Key}: {string.Join(", ", item.Value)}");
        }
    }
    Console.WriteLine();

    // Response summary.
    Console.WriteLine($"HTTP/{response.Version.Major}.{response.Version.Minor} {(int) response.StatusCode} {response.ReasonPhrase}");
    Console.WriteLine();

    // Response headers.
    foreach (var item in response.Headers) {
        Console.WriteLine($"{item.Key}: {string.Join(", ", item.Value)}");
    }
    if (response.Content != null) {
        foreach (var item in response.Content.Headers) {
            Console.WriteLine($"{item.Key}: {string.Join(", ", item.Value)}");
        }
    }

    return Task.CompletedTask;
}
```


# Example: Logging Detailed Response Information with Timing Information

In this example, we extend the previous example to include information about how long an HTTP request took. Rather than specifying a delegate to invoke before sending a request or after receiving a response, we specify a delegate that can invoke the inner handlers in the request pipeline, and then perform additional work once the response has been received:

```csharp
public void ConfigureServices(IServiceCollection services) {
    services
        .AddHttpClient("Test", options => {
            options.BaseAddress = new Uri("https://some-remote-site.com");
        })
        .AddHttpMessageHandler(() => new Jaahas.Http.HttpRequestPipelineHandler(LogResponseDetailsWithTiming));
}


private static async Task LogResponseDetailsWithTiming(HttpRequestMessage request, HttpMessageHandlerDelegate next, CancellationToken cancellationToken) {
    // Start the stopwatch.
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    // Invoke the inner handlers.
    var response = await next(request, cancellationToken).ConfigureAwait(false);

    // Measure the elapsed time.
    var elapsed = stopwatch.ElapsedMilliseconds;

    // Method and endpoint.
    Console.WriteLine($"{response.RequestMessage.Method} {response.RequestMessage.RequestUri}");
    Console.WriteLine();

    // Request headers.
    foreach (var item in response.RequestMessage.Headers) {
        Console.WriteLine($"{item.Key}: {string.Join(", ", item.Value)}");
    }
    if (response.RequestMessage.Content != null) {
        foreach (var item in response.RequestMessage.Content.Headers) {
            Console.WriteLine($"{item.Key}: {string.Join(", ", item.Value)}");
        }
    }
    Console.WriteLine();

    // Response summary and elapsed time.
    Console.WriteLine($"HTTP/{response.Version.Major}.{response.Version.Minor} {(int) response.StatusCode} {response.ReasonPhrase}");
    Console.WriteLine($"{elapsed} ms");
    Console.WriteLine();

    // Response headers.
    foreach (var item in response.Headers) {
        Console.WriteLine($"{item.Key}: {string.Join(", ", item.Value)}");
    }
    if (response.Content != null) {
        foreach (var item in response.Content.Headers) {
            Console.WriteLine($"{item.Key}: {string.Join(", ", item.Value)}");
        }
    }
}
```
