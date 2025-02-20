Create pack:
	dotnet pack -c Release -p:NuspecFile=Pompo.nuspec

Publish pack:
	nuget add bin\Release\Pompo.1.0.0.nupkg -source ...