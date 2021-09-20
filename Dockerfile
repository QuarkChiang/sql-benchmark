FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /src
COPY . .
RUN dotnet restore "./SQLBenchmark.csproj" && dotnet publish "SQLBenchmark.csproj" -c Release
ENTRYPOINT ["dotnet", "/src/bin/Release/net5.0/SQLBenchmark.dll"]