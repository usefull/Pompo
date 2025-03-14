using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WasmModule;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddTransient<UtilityService>();

var host = builder.Build();

await host.UsePompoAsync();

await host.RunAsync();