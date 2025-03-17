using Microsoft.JSInterop;

namespace Pompo
{
    public partial class Factory
    {
        [JSInvokable]
        public async Task Create_demo(string id)
        {
            var obj = new WasmModule.DemoService(id);
            await TransmitObject(obj);
        }
    }
}