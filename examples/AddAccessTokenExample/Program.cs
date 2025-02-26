using System;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("Test")
    .AddHttpMessageHandler(() => new Jaahas.Http.HttpRequestPipelineHandler((request, next, ct) => {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Guid.NewGuid().ToString());
        return next.Invoke(request, ct);
    }));

var app = builder.Build();

app.Run();
