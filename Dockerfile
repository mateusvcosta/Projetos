FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["C#/TunaEngine.Api/TunaEngine.Api.csproj", "C#/TunaEngine.Api/"]
RUN dotnet restore "C#/TunaEngine.Api/TunaEngine.Api.csproj"

COPY ["C#/TunaEngine.Api/", "C#/TunaEngine.Api/"]
RUN dotnet publish "C#/TunaEngine.Api/TunaEngine.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 10000
CMD ["sh", "-c", "dotnet TunaEngine.Api.dll --urls http://0.0.0.0:${PORT:-10000}"]
