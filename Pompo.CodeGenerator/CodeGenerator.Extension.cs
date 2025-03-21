﻿using System;

namespace Pompo
{
    /// <summary>
    /// Generates source code for WebAssembly host extension method.
    /// </summary>
    public partial class CodeGenerator
    {
        /// <summary>
        /// Generates WebAssembly host extension source code.
        /// </summary>
        /// <returns>Source code.</returns>
        private string GenerateWebAssemblyHostExtensionCode() => $@"/// <summary>
/// The extension method for WebAssemblyHost.
/// Generated by Pompo {DateTime.Now}.
/// </summary>

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

namespace Pompo
{{
    public static class WebAssemblyHostExtension
    {{
        public static async Task UsePompoAsync(this WebAssemblyHost host)
        {{
            var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
            var jsTransmitModule = await jsRuntime.InvokeAsync<IJSObjectReference>(""import"", ""./_pompo.js"");
            var factory = DotNetObjectReference.Create(new Factory(jsTransmitModule, host.Services));
            await jsTransmitModule.InvokeVoidAsync(""transmit"", factory);
        }}
    }}
}}";
    }
}