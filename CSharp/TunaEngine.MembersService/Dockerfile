FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TunaEngine.MembersService.csproj", "./"]
RUN dotnet restore "TunaEngine.MembersService.csproj"
COPY . .
RUN dotnet publish "TunaEngine.MembersService.csproj" --configuration Release --output /app/publish --no-restore /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 10001
CMD ["sh", "-c", "dotnet TunaEngine.MembersService.dll --urls http://0.0.0.0:${PORT:-10001}"]
