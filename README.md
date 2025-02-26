# About

Jaahas.HttpRequestTransformer defines delegating handlers for transforming HTTP requests and responses.


# Transforming Requests and Responses via Delegates

The `HttpRequestPipelineHandler` delegating handler can be used to transform requests and responses via a delegate:

```csharp
builder.Services.AddHttpClient("PipelineTransformer")
    .AddHttpMessageHandler(() => new HttpRequestPipelineHandler(async (request, next, cancellationToken) => {
        request.Headers.Add("X-CustomRequestHeader", Guid.NewGuid().ToString());
        var response = await next.Invoke(request, cancellationToken);
        response.Headers.Add("X-CustomResponseHeader", Guid.NewGuid().ToString());
        return response;
    }));
```


# Compressing Outgoing Requests

Jaahas.HttpRequestTransformer provides delegating handlers that can compress outgoing requests (for example if you are sending a large file or other payload to a server that supports compressed request bodies).

## GZip

The `GZipCompressor` delegating handler can be used to compress outgoing requests using GZip compression:

```csharp
builder.Services.AddHttpClient("GZipCompressor")
    .AddHttpMessageHandler(() => new GZipCompressor());
```

The constructor also accepts an optional callback that can be used to determine whether a request should be compressed or not:

```csharp
builder.Services.AddHttpClient("GZipCompressor")
    // Compress requests to specific routes
    .AddHttpMessageHandler(() => new GZipCompressor(callback: request => request.RequestUri.LocalPath.Contains("/api/upload"))));
```


## Brotli

> Brotli compression is not available when targeting .NET Framework.

The `BrotliCompressor` delegating handler can be used to compress outgoing requests using [Brotli](https://developer.mozilla.org/en-US/docs/Glossary/Brotli_compression) compression:

```csharp
builder.Services.AddHttpClient("BrotliCompressor")
    .AddHttpMessageHandler(() => new BrotliCompressor());
```

The constructor also accepts an optional callback that can be used to determine whether a request should be compressed or not:

```csharp
builder.Services.AddHttpClient("BrotliCompressor")
    // Compress requests to specific routes
    .AddHttpMessageHandler(() => new BrotliCompressor(callback: request => request.RequestUri.LocalPath.Contains("/api/upload")));
```


# Creating Custom HTTP Request Pipelines

> In most cases, you should use Microsoft.Extensions.Http to configure and create HTTP clients, especially for long-running applications that need to manage the lifecycle of HTTP message handlers. The `HttpClientFactory` class in this library is designed for simple use cases where dependency injection is not available.

The `HttpClientFactory` class can be used to simplify creation of HTTP clients and HTTP pipelines where one or more delegating handlers are required:

```csharp
var httpClient = HttpClientFactory.Create(
    new HttpRequestPipelineHandler((request, next, cancellationToken) => {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
        return next.Invoke(request, cancellationToken);
    }),
    new GZipCompressor());
```

# Building the Solution

The repository uses [Cake](https://cakebuild.net/) for cross-platform build automation. The build script allows for metadata such as a build counter to be specified when called by a continuous integration system such as TeamCity.

A build can be run from the command line using the [build.ps1](/build.ps1) PowerShell script or the [build.sh](/build.sh) Bash script. For documentation about the available build script parameters, see [build.cake](/build.cake).


# Software Bill of Materials

To generate a Software Bill of Materials (SBOM) for the repository in [CycloneDX](https://cyclonedx.org/) XML format, run [build.ps1](./build.ps1) or [build.sh](./build.sh) with the `--target BillOfMaterials` parameter.

The resulting SBOM is written to the `artifacts/bom` folder.
