using Microsoft.CodeAnalysis;
using Microsoft.JSInterop;
using Pompo;

namespace PompoTestWasm
{
    [Alias("MainClient")]
    public partial class SomeClient
    {
        private int _i;

        //[Alias("Rem")]
        public SomeClient()
        {
            _i = 0;
        }

       
        public SomeClient(int i, string i)
        {
            _i = 0;
        }

        [JSInvokable]
        
        public void Return(int i, string str) { }

        [JSInvokable]
        
        public void Term(int i, string str) { }
    }
}
