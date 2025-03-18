using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Pompo;
using System.Text.Json;

namespace WasmModule
{
    [PompoAlias("fromdi")]
    public class DiService(UtilityService utilityService)
    {
        private readonly UtilityService _ctorInitializedSvc = utilityService;

        [Inject]
        public UtilityService? InjectedSvc { get; set; }

        [JSInvokable("check")]
        public void Checking(JsonElement data)
        {
            Console.WriteLine($"Initialized via ctor service: {_ctorInitializedSvc.GetCreationTime()}");
            Console.WriteLine($"Injected via property service: {InjectedSvc?.GetCreationTime()}");
            Console.WriteLine($"data: prompt - {data.GetProperty("prompt").GetString()}, val - {data.GetProperty("val").GetDouble()}");
        }
    }
}