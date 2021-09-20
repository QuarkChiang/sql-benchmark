FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SQLBenchmark.csproj", "."]
RUN dotnet restore "./SQLBenchmark.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "SQLBenchmark.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SQLBenchmark.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SQLBenchmark.dll"]