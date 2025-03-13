using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Pompo;
using System.Runtime.CompilerServices;
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

        [Inject]
        public string HttpClient { get; set; }

        public static void Inject(object obj, IServiceProvider sp)
        {
            var props = obj.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(typeof(InjectAttribute), true).Any());

            foreach (var p in props)
            {
                var s = sp.GetService(p.GetGetMethod().ReturnType)
                    ?? throw new ApplicationException($"Unable resolve service {p.GetGetMethod().ReturnType.FullName} for property {obj.GetType().FullName}.{p.Name}");
                p.SetValue(obj, s);
            }
        }
    }

    public class Selector
    {
        public string Name { get; set; }

        public double Value { get; set; }
    }
}