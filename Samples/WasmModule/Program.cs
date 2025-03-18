using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WasmModule;
using Pompo;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddTransient<UtilityService>();
builder.Services.AddTransient<DiService>();

var host = builder.Build();

await host.UsePompoAsync();

await host.RunAsync();