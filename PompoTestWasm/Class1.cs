using Microsoft.JSInterop;

namespace PompoTestWasm
{
    public partial class SomeClient
    {
        public SomeClient(string i)
        {
            int.TryParse(i, out _i);
        }

        [JSInvokable]

        public void Term2(int i, string str) { }
    }
}
