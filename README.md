# Pompo
The brige to create and use .NET objects in JS code via WebAssembly.

## Intro
We are going to create a WebAssembly module using the Blazor framework and the C# programming language. Then we will create a React application and embed the WebAssembly module in it so that we can use the functionality of the module in the application.

## Creating a WebAssembly Module
1. In the Visual Studio create Blazor WebAssembly Standalone App. Name it _WasmModule_.
2. We don't need any pages in this project, it will contain only the functional service. So clear the _wwwroot_ folder. Remove _Layout_ and _Pages_ folders. Remove __Imports.razor_ and _App.razor_ files.
3. Remove the _Microsoft.AspNetCore.Components.WebAssembly.DevServer_ package reference.
4. Add _Pompo_ package reference:
```
nuget install Pompo
```
5. Add the service class:
```cs
using Microsoft.JSInterop;
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
                await Task.Delay(TimeSpan.FromSeconds(rnd.Next(1, 3)));
                progress += rnd.Next(5, 20);
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
            response.serviceId = _id;
            return response;
        }
    }

    public class SumResponce
    {
        public string? serviceId { get; set; }

        public int x { get; set; }

        public int y { get; set; }

        public int sum { get; set; }

        public void Calculate() => sum = x + y;
    }
}
```
As you can see, the service class is marked with an attribute _PompoAlias_ with a parameter _"demo"_. This means that the service will be available in the JS under the _demo_ name. The _PompoAlias_ attribute is otional. If the alias is not specified, the service name in JS will look like _{NAMESPACE}__{CLASSNAME}_.
Class methods that are available for calling from JS are marked with the _JSInvokable_ attribute. The attribute parameter specifies the name of the method by which it will be available in JS. If the parameter is not specified, the method will be available by its real name. A class that has no JSInvokable methods will not be accessible in JS.

8. Edit Program.cs.
```cs
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WasmModule;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var host = builder.Build();

await host.UsePompoAsync();

await host.RunAsync();
```
Here we just create the WebAssembly host, initialize the Pompo JS factory and launch the host.
9. Build the application.
10. Create the folder publish profile with default parameters and publish the application.

## Creating a React application and embeding in it the WebAssembly module
1. Create a React application:
```
npx create-react-app@latest react-app
```
2. Copy the __framework_ folder and the __pompo.js_ file from the WebAsswmbly module publish folder to the _public_ folder of the React application.
3. Add tags in the _head_ section of the _index.html_ file in the _public_ folder of the React application:
```html
<head>
   ...
   <base href="/" />
   <script src="_framework/blazor.webassembly.js"></script>
</head>
```
4. Run the React application:
```
npm start
```
If everything is fine, a message will appear in the browser console:
```
Pompo factory initialized.
```
5. Now we have a factory available to create an instance of our service implemented in the WebAssembly module:
```js
let demo = await window.dotNetObjectFactory.create_demo('foo');
```
As you can see, to create the instance of the service, we use the _create_demo_ method. The method takes one argument, just like the service constructor.
Method names for creating objects are formed as follows: _create__{SERVICENAME}_. _SERVICENAME_ is either an alias defined in the _PompoAlias_ attribute or a real class name with namespace.
For example, if we didn't use the _PompoAlias_ attribute for the _DemoService_ class, the name of the method to create an instance of the service would look like this: _create__WasmModule__DemoService_.
In any case, you can always look into the __pompo.js_ file and find out the name of a particular method.

## In conclusion
Sample projects for the WebAssembly module and the React application can be found in the _Samples_ folder of this repo.
