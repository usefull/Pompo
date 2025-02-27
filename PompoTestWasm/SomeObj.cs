using Microsoft.JSInterop;
using Pompo;

namespace Pompo1;


    public class SomeClient
    {
        [JSInvokable]

        public void Do(int i, string str) { }
    }

