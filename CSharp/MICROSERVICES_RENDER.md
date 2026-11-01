# TunaEngine: serviços independentes no Render

A API foi separada em quatro aplicações ASP.NET Core independentes:

| Serviço | Responsabilidade | Porta local |
|---|---|---:|
| `TunaEngine.MembersService` | membros, login e cargos | 10001 |
| `TunaEngine.EventsService` | eventos | 10002 |
| `TunaEngine.DocumentsService` | documentos e tipos de documento | 10004 |
| `TunaEngine.InventoryService` | inventário | 10003 |

Cada aplicação tem um projeto `.csproj`, um `Dockerfile` e um endpoint
`GET /health`. As rotas funcionais foram mantidas, por exemplo:

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
```

É necessário definir `MongoDb__ConnectionString` no ambiente de cada processo.
As restantes configurações têm valores por omissão compatíveis com a base de
dados existente.

## Configuração no Render

Criar quatro serviços do tipo **Web Service** com runtime **Docker**, ligados ao
mesmo repositório:

1. `tunaengine-members`
   - Root Directory: `CSharp/TunaEngine.MembersService`
   - Dockerfile: `Dockerfile`
2. `tunaengine-events`
   - Root Directory: `CSharp/TunaEngine.EventsService`
   - Dockerfile: `Dockerfile`
3. `tunaengine-documents`
   - Root Directory: `CSharp/TunaEngine.DocumentsService`
   - Dockerfile: `Dockerfile`
4. `tunaengine-inventory`
   - Root Directory: `CSharp/TunaEngine.InventoryService`
   - Dockerfile: `Dockerfile`

Os nomes acima são os nomes esperados pela aplicação MAUI, que utiliza os
seguintes endereços:

```text
https://tunaengine-members.onrender.com/
https://tunaengine-events.onrender.com/
https://tunaengine-documents.onrender.com/
https://tunaengine-inventory.onrender.com/
```

Se forem escolhidos nomes diferentes no Render, atualizar as constantes em
`TunaEngine/Services/ApiServiceUrls.cs` antes de gerar a aplicação cliente.

Em cada serviço, adicionar a variável:

```text
MongoDb__ConnectionString=<connection string do MongoDB>
```

O Render fornece automaticamente a variável `PORT`. Os Dockerfiles utilizam
essa variável e usam `10001`, `10002` e `10003` apenas como valores locais por
omissão. Não é necessário configurar manualmente uma porta pública fixa.

Depois do primeiro deployment, validar:

```text
https://<members-service>.onrender.com/health
https://<events-service>.onrender.com/health
https://<documents-service>.onrender.com/health
https://<inventory-service>.onrender.com/health
```

O plano gratuito do Render pode suspender serviços sem tráfego. O primeiro
pedido depois dessa suspensão pode demorar mais tempo, sem representar uma
falha da API.

## Bases de dados MongoDB

Os serviços usam a mesma ligação MongoDB, mas bases de dados separadas:

- Members service: base de dados `Members`, com as coleções `Members` e `Roles`;
- Events service: `Events`;
- Documents service: `Documents` e `Doc_Type`;
- Inventory service: `Inventory`.

Esta separação evita que um serviço aceda diretamente às coleções dos outros
domínios e permite migrar cada serviço de forma independente.

## Limitação atual

Os quatro serviços são independentes e podem ser publicados separadamente, mas
não existe ainda um API Gateway. Os clientes devem utilizar o URL do serviço
que disponibiliza cada recurso. Um gateway pode ser acrescentado futuramente
para oferecer um único URL público e encaminhar cada rota para o serviço
correspondente.
