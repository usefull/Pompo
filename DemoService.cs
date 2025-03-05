﻿using Microsoft.JSInterop;
using Pompo;
using System.Text.Json;

namespace WasmModule
{
    [PompoAlias("demo")]
    public class DemoService
    {
        private readonly string _id;

        public DemoService(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            _id = id;

            Console.WriteLine($"DemoService created with id: {_id}");
        }

        [JSInvokable("do")]
        public async Task DoSomeWork()
        {
            var rnd = new Random();
            var progress = 0;

            while (progress < 100)
            {
                await Task.Delay(TimeSpan.FromSeconds(rnd.Next(1, 5)));
                progress += rnd.Next(3, 10);
                if (progress > 100)
                    progress = 100;

                Console.WriteLine($"{_id} work progress: {progress}%");
            }
        }

        [JSInvokable("sum")]
        public SumResponce? Sum(JsonElement request)
        {
            var response = request.Deserialize<SumResponce>();
            response ??= new SumResponce();
            response.Calculate();
            response.ServiceId = _id;
            return response;
        }
    }

    public class SumResponce
    {
        public string? ServiceId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Sum { get; set; }

        public void Calculate() => Sum = X + Y;
    }
}