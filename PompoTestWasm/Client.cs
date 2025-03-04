using Microsoft.JSInterop;
using Pompo;
using System.Text.Json;

namespace WasmModule
{
    [PompoAlias("Worker")]
    public class UsefulWorker(string id)
    {
        private string _id = id;

        public UsefulWorker() : this(string.Empty)
        {
        }

        [JSInvokable]
        public string GetId() => _id;
    }
}

namespace PompoTestWasm
{
    [PompoAlias("FakeClient")]
    public partial class SomeClient
    {
        private string? _name;

        public SomeClient()
        {

        }

        [PompoAlias("NamedClient")]
        public SomeClient(string name)
        {
            _name = name;
        }

        [JSInvokable("Run")]
        public async Task DoSomeWork(int iter)
        {
            if (iter < 1)
                return;

            var rnd = new Random();

            for (var i = 0; i < iter; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(rnd.Next(1, 10)));
                Console.WriteLine($"Iteration {i + 1} done.");
            }
        }

        [JSInvokable]
        public string? PassObject(JsonElement obj) => obj.GetRawText();

        [JSInvokable("get")]
        public Selector GetObject() => new Selector { Name = $"{_name} TETYETT IOIYIY erer 2434 fgf", Value = 1.256 };
    }

    public class Selector
    {
        public string Name { get; set; }

        public double Value { get; set; }
    }
}