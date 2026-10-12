FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TunaEngine.Api.csproj", "./"]
RUN dotnet restore "TunaEngine.Api.csproj"

COPY . .
RUN dotnet publish "TunaEngine.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 10000
CMD ["sh", "-c", "dotnet TunaEngine.Api.dll --urls http://0.0.0.0:${PORT:-10000}"]
