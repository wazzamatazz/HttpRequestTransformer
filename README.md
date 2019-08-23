# HttpRequestTransformer

Contains an `HttpMessageHandler` ([HttpRequestTransformHandler](./src/HttpRequestTransformer/HttpRequestTransformHandler.cs)) that can be used to modify outgoing HTTP requests prior to sending. An example use case includes a backchannel HTTP client in a server-side app where you need to attach the authorization token for the calling user to the request.

# Getting Started

Add the `Jaahas.HttpRequestTransformer` [NuGet package](https://www.nuget.org/packages/Jaahas.HttpRequestTransformer) to your project.

# Example

In this example, we'll attach a bearer token to outgoing requests made by a backchannel client in an ASP.NET Core application. First, add an [HttpRequestTransformHandler](./src/HttpRequestTransformer/HttpRequestTransformHandler.cs) to the request pipeline for your `HttpClient`:

```csharp
public void ConfigureServices(IServiceCollection services) {
    services
        .AddHttpClient("Test", options => {
            options.BaseAddress = new Uri("https://some-remote-site.com");
        })
        .AddHttpMessageHandler(() => new Jaahas.Http.HttpRequestTransformHandler(AddBearerTokenToRequest));
}
```

Next, implement the function that will retrieve the access token for a given `ClaimsPrincipal`:

```csharp
private static async Task AddBearerTokenToRequest(HttpRequestMessage request, CancellationToken cancellationToken) {
    var principal = request.GetStateProperty<ClaimsPrincipal>();
    if (principal?.Identity?.IsAuthenticated ?? false) {
        string token;

        // Plug in your own logic here to get the actual token for the principal...

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
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
