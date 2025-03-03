using Microsoft.JSInterop;
using Pompo;

namespace PompoTestWasm
{
    //[Alias("qwerty")]
    public partial class SomeClient
    {
        private int _i;

        //[Alias("Erm")]
        //public SomeClient()
        //{
        //    _i = 0;
        //}

        //[Alias("Rem")]
        //public SomeClient(int i)
        //{
        //    _i = i;
        //}

        [JSInvokable]
        [Alias("Term1")]      
        public void Return(int i, string str) { }

        [JSInvokable]        
        public void Term(int i, string str) { }
    }
}
