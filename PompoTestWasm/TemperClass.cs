using Microsoft.JSInterop;

namespace PompoTestWasm
{
    public class TemperClass
    {
        public string Time() => "dsfdsfd";
    }
}

namespace PompoTest
{
    public class TttClass
    {
        public string Ttt() => "dsfdsfd";

        [JSInvokable]
        string Ttt1() => "dsfdsfd";
        
    }
}
