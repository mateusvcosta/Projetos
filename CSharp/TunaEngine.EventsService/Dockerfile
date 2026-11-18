FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TunaEngine.EventsService.csproj", "./"]
RUN dotnet restore "TunaEngine.EventsService.csproj"
COPY . .
RUN dotnet publish "TunaEngine.EventsService.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER 1654
ENTRYPOINT ["dotnet", "TunaEngine.EventsService.dll"]
