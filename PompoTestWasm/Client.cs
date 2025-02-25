using Microsoft.CodeAnalysis;
using Microsoft.JSInterop;
using Pompo;

namespace PompoTestWasm
{
    [Alias("  erere  ")]
    public class Client
    {
        private int _i;

        [Alias("ddf")]
        Client(int i)
        {
            _i = i;
        }

        public Client()
        {
            _i = 0;
        }

        [JSInvokable]
        public void Return(int i, string str) { }
    }
}
