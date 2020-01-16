cd %~dp0
dotnet build magic.node/magic.node.csproj --configuration Release
dotnet build magic.node.expressions/magic.node.expressions.csproj --configuration Release
dotnet build magic.node.extensions/magic.node.extensions.csproj --configuration Release
