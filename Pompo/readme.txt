Create pack:
	dotnet pack -c Release -p:NuspecFile=Pompo.nuspec

Publish pack to folder:
	nuget add bin\Release\Pompo.1.2.0.nupkg -source ...

Publish pack to nuget.org:
	dotnet nuget push bin\Release\Pompo.1.2.0.nupkg --api-key ... --source https://api.nuget.org/v3/index.json