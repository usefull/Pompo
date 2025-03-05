using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WasmModule;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var host = builder.Build();

await host.UsePompoAsync();

await host.RunAsync();
