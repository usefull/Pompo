﻿Create pack:
	dotnet pack -c Release -p:NuspecFile=Pompo.nuspec

Publish pack to folder:
	nuget add bin\Release\Pompo.1.1.0.nupkg -source ...

Publish pack to nuget.org: