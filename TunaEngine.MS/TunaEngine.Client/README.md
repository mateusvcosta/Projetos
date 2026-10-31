# TunaEngine client for the microservices API

This is a separate .NET MAUI copy of the original client. The original `TunaEngine` app and its API configuration are unchanged.

## Run the services

From the `TunaEngine.Microservices` folder, start the APIs and MongoDB:

```powershell
docker compose up --build -d
```

## Run the app

Open `TunaEngine.Microservices.Client.csproj` in Visual Studio and run the Windows target. The app connects to the local service ports:

- Auth: `http://localhost:8081`
- Events: `http://localhost:8082`
- Inventory: `http://localhost:8083`
- Members: `http://localhost:8084`

On the Android emulator, the app uses `10.0.2.2` to reach the host computer. For a physical device, change `apiHost` in `MauiProgram.cs` to the computer's address on the local network.

The login page can register a new account through Auth, then save or load its profile through Members. The client sends the JWT to protected service endpoints. Inventory and event data are read and written through their respective APIs; event cards can request an IPMA forecast.

## Current API coverage

The microservices currently expose authentication, a member's own profile, inventory, and events. This client copy uses those areas. The microservices do not yet expose a team-member listing, roles administration, or documents, so those screens are omitted from this copy's navigation.
