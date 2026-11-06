# OpenAPI — TunaEngine

O ficheiro [`openapi.yaml`](openapi.yaml) descreve as rotas HTTP atualmente expostas pelo API Gateway, os esquemas JSON, os campos obrigatórios e as respostas documentadas.

## Consultar a especificação

Importe `openapi.yaml` num visualizador compatível com OpenAPI 3.0, como Swagger Editor, ou numa coleção/ferramenta de API que aceite ficheiros OpenAPI.

Os servidores indicados no ficheiro são:

- Render: `https://tunaengine-gateway.onrender.com` — endereço configurado no cliente; a disponibilidade atual não foi confirmada.
- Docker local: `http://localhost:10000` — porta de fallback do container do Gateway.

## Limites conhecidos da implementação

- O Gateway não serve atualmente um endpoint OpenAPI nem uma interface Swagger. A especificação é um artefacto estático versionado com o código.
- A API não implementa autenticação Bearer/JWT. O login valida credenciais, mas não emite token.
- `file_Path` é metadado e não corresponde a upload de ficheiros.
- A atualização de membro aceita alguns campos que o serviço não persiste; esse comportamento está assinalado no OpenAPI.
- Erros de validação usam `ValidationProblemDetails`; IDs MongoDB inválidos devolvem um objeto `{ "error": "..." }`.

Atualize esta especificação quando forem alterados controllers, DTOs ou a configuração de rotas do Gateway.
