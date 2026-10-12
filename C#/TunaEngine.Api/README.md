# TunaEngine API

ASP.NET Core API for storing and managing the inventory documents in MongoDB. The API runs separately from the .NET MAUI app so the database connection string stays on the server.

## Configure and run locally

Set the connection string in the environment instead of committing credentials:

```powershell
$env:MongoDb__ConnectionString = "mongodb://localhost:27017"
dotnet run --project .\TunaEngine.Api.csproj
```

For MongoDB Atlas, set `MongoDb__ConnectionString` to the connection string supplied by Atlas. `MongoDb:DatabaseName` and `MongoDb:InventoryCollectionName` default to `TunaEngine` and `inventory`; override them with `MongoDb__DatabaseName` and `MongoDb__InventoryCollectionName`.

The singleton `MongoClient` is reused for the lifetime of the API. Pool and timeout options intentionally use the MongoDB driver's defaults until deployment topology and workload concurrency are known.

Check `GET /health` to verify the API can ping MongoDB. Example requests are in `TunaEngine.Api.http`.

## Inventory endpoints

| Method | Route | Result |
| --- | --- | --- |
| `GET` | `/api/inventory` | List inventory items |
| `GET` | `/api/inventory/{id}` | Get one item by MongoDB ObjectId |
| `POST` | `/api/inventory` | Create an item |
| `PUT` | `/api/inventory/{id}` | Update an item's editable fields |
| `DELETE` | `/api/inventory/{id}` | Delete an item |

Create/update JSON uses `designation` (required), `instrumentType`, `price`, `condition`, `function`, and `notes`. New documents also receive a MongoDB ObjectId, an `INV-...` inventory code, and the current UTC date.

The API currently has no caller authentication. Keep it on localhost or a trusted network; add authentication and HTTPS before exposing it publicly. A MAUI client should call this API's reachable server URL and must never contain the MongoDB connection string.
