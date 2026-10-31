# TunaEngine Microservices Prototype

This folder is a small, runnable microservices slice for the TunaEngine project. It leaves the original project untouched.

## Services

- **Auth API** owns user registration, password hashes, and short-lived JWTs. It stores its data in the `tuna_auth` database.
- **Members API** owns member profiles, keyed by the authenticated user's id. It stores its data in `tuna_members`.
- **Events API** owns event records and attendance. It stores its data in the separate `tuna_events` database and requires a JWT issued by Auth API.
- **Inventory API** owns inventory items and generated inventory codes. It stores its data in `tuna_inventory`.
- **MongoDB** is shared infrastructure, but each service uses its own database and collection.

The services are separate ASP.NET applications with separate Docker images, ports, and data stores. They validate the token contract produced by Auth. Events also calls Inventory over REST to reject event requests that reference missing inventory items, and calls IPMA Open Data to retrieve the forecast for an event's locality and date.

See [`architecture.mmd`](architecture.mmd) for the architecture diagram source.

## Run locally with Docker Compose

1. Install/start Docker Desktop.
2. Copy `.env.example` to `.env`. The included value is only for local development; use a private random secret outside this prototype.
3. From this folder, run `docker compose up --build`.
4. Open the OpenAPI documents:
   - Auth: `http://localhost:8081/openapi/v1.json`
   - Events: `http://localhost:8082/openapi/v1.json`
   - Inventory: `http://localhost:8083/openapi/v1.json`
   - Members: `http://localhost:8084/openapi/v1.json`
5. Import `postman/TunaEngine-Microservices.postman_collection.json` into Postman and run its requests in order.

The weather integration uses IPMA's public daily forecast endpoint. Events store the IPMA `GlobalIdLocal` locality code with the event; for example, Coimbra is `1060300`. Find the correct code in [IPMA's locality list](https://api.ipma.pt/open-data/distrits-islands.json) before creating an event. Forecast responses are cached in memory for six hours. IPMA publishes forecasts for a limited horizon (up to five days); requests outside the available dates return `404`.

Health endpoints are available at `/health` on each service port.

## Demo flow

1. Register a member at `POST /api/auth/register`.
2. Log in at `POST /api/auth/login` and receive a one-hour bearer token.
3. Save the member's profile at `PUT /api/members/me`.
4. Create an inventory item at `POST /api/inventory`.
5. Create an event at `POST /api/events`, referring to the inventory item id and setting its `globalIdLocal` to an IPMA locality code. Events calls Inventory over REST to verify the item exists.
6. Confirm or decline attendance at `POST /api/events/{id}/attendance`.
7. Retrieve the event forecast at `GET /api/events/{id}/weather`.
8. Retrieve the event and check its participant status.

The attendance identity comes from the authenticated token, so the caller cannot submit a different member ID in the request body.

## Environment values

Compose reads `JWT_SECRET` from `.env`. Both APIs must use the same value. The secret must be at least 32 bytes. MongoDB connection strings and database names are supplied to each API as environment variables.

## Scope and limitations

This prototype demonstrates service separation, independent deployment, API contracts, MongoDB ownership by service, JWT authentication, REST integration, an event/inventory workflow, and an IPMA weather integration. It does not implement the full TunaEngine requirements: role/permission administration, multi-tuna tenancy, map and Google Calendar integrations from the BPMN, production TLS, cloud deployment, or a message broker. IPMA asks applications using its data to notify `webmaster@ipma.pt` about the use and its purpose; see the [IPMA API documentation](https://api.ipma.pt/).

The JWT implementation follows the authentication pattern in the instructor's sample, with hashed passwords and token validation. The sample's `.env` and hard-coded Compose secret were intentionally not copied. The sample's password-change route was also omitted because its handler has undefined variables.

Passwords in this C# version use ASP.NET Identity's versioned password hasher. If the course rubric requires bcrypt or Argon2 specifically, that algorithm remains a follow-up; the supplied Node sample uses bcryptjs but needs repair before reuse.
