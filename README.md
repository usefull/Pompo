# Pompo
The brige to create and use .NET objects in JS code via Wasm.

1. Create the standalone Blazor WebAssembly app.
2. Clean the wwwroot folder.
3. Remove the Microsoft.AspNetCore.Components.WebAssembly.DevServer package reference.
4. Remove folders Layout and Pages and their contents.
5. Remove files _Imports.razor and App.razor.
6. Add Pompo package reference.
7. Create service class.
8. Edit Program.cs.