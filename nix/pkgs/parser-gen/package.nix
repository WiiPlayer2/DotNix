{
  buildDotnetModule,
  dotnetCorePackages,
}:
buildDotnetModule {
  pname = "parser-gen";
  version = "0.1";
  src = ../../../Tools/ParserGen;

  projectFile = "ParserGen.csproj";
  dotnet-sdk = dotnetCorePackages.sdk_10_0;
  dotnet-runtime = dotnetCorePackages.runtime_10_0;

  nugetDeps = ./deps.json;
}
