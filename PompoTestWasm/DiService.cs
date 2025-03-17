using Microsoft.JSInterop;
using Pompo;
using WasmModule;

namespace PompoTestWasm
{
    [PompoAlias("FromDI")]
    public class DiService
    {
        private readonly UtilityService _utility;

        public DiService(UtilityService utility)
        {
            _utility = utility;
        }

        [JSInvokable]
        public void MakeMe(string prompt)
        {
            Console.WriteLine(prompt);
            Console.WriteLine($"from Utility: {_utility.GetCreationTime()}");
        }
    }
}