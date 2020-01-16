
set version=%1
set key=%2

cd %~dp0

dotnet build magic.node/magic.node.csproj --configuration Release --source https://api.nuget.org/v3/index.json
dotnet nuget push magic.node/bin/Release/magic.node.%version%.nupkg -k %key% -s https://api.nuget.org/v3/index.json

dotnet build magic.node.expressions/magic.node.expressions.csproj --configuration Release --source https://api.nuget.org/v3/index.json
dotnet nuget push magic.node.expressions/bin/Release/magic.node.expressions.%version%.nupkg -k %key% -s https://api.nuget.org/v3/index.json

dotnet build magic.node.extensions/magic.node.extensions.csproj --configuration Release --source https://api.nuget.org/v3/index.json
dotnet nuget push magic.node.extensions/bin/Release/magic.node.extensions.%version%.nupkg -k %key% -s https://api.nuget.org/v3/index.json

