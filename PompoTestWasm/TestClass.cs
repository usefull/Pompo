using Microsoft.JSInterop;

namespace PompoTestWasm
{
    public partial class TestClass
    {
        [JSInvokable]
        public string Method(string str, int n) => "334";

        public string Funct() => "9899";

        protected void Prot() { }
    }
}
