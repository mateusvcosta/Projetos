# TunaEngine: serviços independentes no Render

A API foi separada em quatro aplicações ASP.NET Core independentes e um API
Gateway:

| Serviço | Responsabilidade | Porta local |
|---|---|---:|
| `TunaEngine.MembersService` | membros, login e cargos | 10001 |
| `TunaEngine.EventsService` | eventos | 10002 |
| `TunaEngine.DocumentsService` | documentos e tipos de documento | 10004 |
| `TunaEngine.InventoryService` | inventário | 10003 |
| `TunaEngine.Gateway` | ponto de entrada e encaminhamento | 10000 |

Cada aplicação tem um projeto `.csproj`, um `Dockerfile` e um endpoint
`GET /health`. As rotas funcionais são:

```text
/api/members
/api/roles
/api/events
/api/documents
/api/doc-types
/api/inventory
```

## Configuração local

Cada serviço pode ser executado a partir da respetiva pasta:

```powershell
dotnet run --project .\TunaEngine.MembersService
dotnet run --project .\TunaEngine.EventsService
dotnet run --project .\TunaEngine.DocumentsService
dotnet run --project .\TunaEngine.InventoryService
dotnet run --project .\TunaEngine.Gateway
```

É necessário definir `MongoDb__ConnectionString` no ambiente de cada
microservice. O Gateway não acede ao MongoDB.

## Configuração no Render

Criar cinco serviços do tipo **Web Service**, com runtime **Docker**, ligados
ao mesmo repositório:

1. `tunaengine-membersservice`
   - Root Directory: `CSharp/TunaEngine.MembersService`
   - Dockerfile Path: `Dockerfile`
   - Docker Build Context Directory: `.`
2. `tunaengine-eventsservice`
   - Root Directory: `CSharp/TunaEngine.EventsService`
   - Dockerfile Path: `Dockerfile`
   - Docker Build Context Directory: `.`
3. `tunaengine-documentsservice`
   - Root Directory: `CSharp/TunaEngine.DocumentsService`
   - Dockerfile Path: `Dockerfile`
   - Docker Build Context Directory: `.`
4. `tunaengine-inventoryservice`
   - Root Directory: `CSharp/TunaEngine.InventoryService`
   - Dockerfile Path: `Dockerfile`
   - Docker Build Context Directory: `.`
5. `tunaengine-gateway`
   - Root Directory: `CSharp/TunaEngine.Gateway`
   - Dockerfile Path: `Dockerfile`
   - Docker Build Context Directory: `.`

Nos quatro microservices, adicionar:

```text
MongoDb__ConnectionString=<connection string do MongoDB>
```

Não adicionar esta variável ao Gateway. O Render fornece automaticamente a
variável `PORT`, utilizada por todos os Dockerfiles.

O cliente MAUI deve utilizar apenas:

```text
https://tunaengine-gateway.onrender.com/
```

Se o Gateway tiver outro nome, atualizar `ApiServiceUrls.Gateway` em
`TunaEngine/Services/ApiServiceUrls.cs` antes de gerar a aplicação cliente.

Os destinos predefinidos do Gateway são:

```text
https://tunaengine-membersservice.onrender.com/
https://tunaengine-eventsservice.onrender.com/
https://tunaengine-documentsservice.onrender.com/
https://tunaengine-inventoryservice.onrender.com/
```

Se algum microservice tiver outro URL, configurar no Gateway a variável
correspondente: `MembersServiceUrl`, `EventsServiceUrl`,
`DocumentsServiceUrl` ou `InventoryServiceUrl`.

## Validação

Validar cada serviço diretamente:

```text
https://tunaengine-membersservice.onrender.com/health
https://tunaengine-eventsservice.onrender.com/health
https://tunaengine-documentsservice.onrender.com/health
https://tunaengine-inventoryservice.onrender.com/health
```

Validar o Gateway:

```text
https://tunaengine-gateway.onrender.com/health
```

O Gateway encaminha `/api/members` e `/api/roles` para Members, `/api/events`
para Events, `/api/documents` e `/api/doc-types` para Documents, e
`/api/inventory` para Inventory.

## Bases de dados MongoDB

Os serviços usam a mesma ligação MongoDB, mas bases de dados separadas:

- Members: base de dados `Members`, com as coleções `Members` e `Roles`;
- Events: base de dados `Events`;
- Documents: bases de dados `Documents` e `Doc_Type`;
- Inventory: base de dados `Inventory`.

Esta separação evita que um serviço aceda diretamente às coleções dos outros
domínios e permite atualizar cada serviço de forma independente.

O plano gratuito do Render pode suspender serviços sem tráfego. O primeiro
pedido depois dessa suspensão pode demorar mais tempo.
