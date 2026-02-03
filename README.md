# Pompo
The brige to create and use .NET objects in JS code via WebAssembly.

## Intro
We are going to create a WebAssembly module using the Blazor framework and the C# programming language. Then we will create a React application and embed the WebAssembly module in it so that we can use the functionality of the module in the application.

## Creating a WebAssembly Module
1. In the Visual Studio create Blazor WebAssembly Standalone App. Name it _WasmModule_.
2. We don't need any pages in this project, it will contain only the functional service. So clear the _wwwroot_ folder. Remove _Layout_ and _Pages_ folders. Remove __Imports.razor_ and _App.razor_ files.
3. Remove the _Microsoft.AspNetCore.Components.WebAssembly.DevServer_ package reference.
4. Make sure the _OverrideHtmlAssetPlaceholders_ property in the project file is set to _false_.
5. Add _Pompo_ package reference:
```
nuget install Pompo
```
6. Add the service class:
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
As you can see, the service class is marked with an attribute _PompoAlias_ with a parameter _"demo"_. This means that the service will be available in the JS under the _demo_ name. The _PompoAlias_ attribute is optional. If the alias is not specified, the service name in JS will look like _{NAMESPACE}__{CLASSNAME}_.
Class methods that are available for calling from JS are marked with the _JSInvokable_ attribute. The attribute parameter specifies the name of the method by which it will be available in JS. If the parameter is not specified, the method will be available by its real name.

7. Edit Program.cs.
```cs
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Pompo;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var host = builder.Build();

await host.UsePompoAsync();

await host.RunAsync();
```
Here we just create the WebAssembly host, initialize the Pompo JS factory and launch the host.

8. Build the application.
9. Create the folder publish profile with default parameters and publish the application.

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

## Explicit .NET object creation from JS code.
Now we have a factory available to create an instance of our service implemented in the WebAssembly module:
```js
let demo = await window.dotNetObjectFactory.create_demo('foo');
```
As you can see, to explicit create the instance of the service, we use the _create_demo_ method. The method takes one argument, just like the service constructor.
Method names for creating objects are formed as follows: *create_{SERVICENAME}*. _SERVICENAME_ is either an alias defined in the _PompoAlias_ attribute or a real class name with namespace.
For example, if we didn't use the _PompoAlias_ attribute for the _DemoService_ class, the name of the method to create an instance of the service would look like this: *create_WasmModule_DemoService*.
In any case, you can always look into the __pompo.js_ file and find out the name of a particular method.

## Resolving services from DI
You can get a service object from the DI container. In order to have this opportunity, you need to register the service in DI.
Let's create another service class:
```cs
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Pompo;
using System.Text.Json;

namespace WasmModule
{
    [PompoAlias("fromdi")]
    public class DiService(UtilityService utilityService)
    {
        private readonly UtilityService _ctorInitializedSvc = utilityService;

        [Inject]
        public UtilityService? InjectedSvc { get; set; }

        [JSInvokable("check")]
        public void Checking(JsonElement data)
        {
            Console.WriteLine($"Initialized via ctor service: {_ctorInitializedSvc.GetCreationTime()}");
            Console.WriteLine($"Injected via property service: {InjectedSvc?.GetCreationTime()}");
            Console.WriteLine($"data: prompt - {data.GetProperty("prompt").GetString()}, val - {data.GetProperty("val").GetDouble()}");
        }
    }
}
```
and one more:
```cs
namespace WasmModule
{
    public class UtilityService
    {
        private readonly DateTime _creation;

        public UtilityService()
        {
            _creation = DateTime.Now;
            Console.WriteLine("Utility service has created");
        }

        public DateTime GetCreationTime() => _creation;
    }
}
```
Insert lines in the _Program.cs_ file before the host building:
```cs
builder.Services.AddTransient<UtilityService>();
builder.Services.AddTransient<DiService>();
```
Then in React application we can get the *DiService* object like this:
```js
let diService = await window.dotNetObjectFactory.resolve_di('fromdi');
```

## Injecting services
Using the *Inject* attribute, it is possible to inject some service into the property of other service. Mark the property with an attribute (as done above in the _DiService_ code):
```cs
[Inject]
public UtilityService? InjectedSvc { get; set; }
```
Dependency injection via constructor parameters is also available. Of course, the service being injected must also be registered in DI.
More info about using DI see [here](https://learn.microsoft.com/ru-ru/dotnet/core/extensions/dependency-injection).

## In conclusion
Sample projects for the WebAssembly module and the React application can be found in the _Samples_ folder of [this repo](https://github.com/usefull/Pompo).