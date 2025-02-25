using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("Test")
    .AddHttpMessageHandler(() => new Jaahas.Http.HttpRequestPipelineHandler((request, next, ct) => {
        request.Headers.Add("X-Test-Header", Guid.NewGuid().ToString());
        return next.Invoke(request, ct);
    }));

var app = builder.Build();

app.Run();
